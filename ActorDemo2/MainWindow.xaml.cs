using ActorDemo2.Logging;
using ActorDemo2.Model;
using SimulationCore.SimulationClasses;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WPFVisualizationBase;

namespace ActorDemo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _simulationSpeed = 32;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindowOnLoad;

            DatePickerStartDate.SelectedDate = DateTime.Now;
            DatePickerEndDate.SelectedDate = DateTime.Now.AddMonths(1);

            DrawingSystem = new DrawingOnCoordinateSystem(0, 300)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            DrawingSystem.SetValue(Grid.RowProperty, 1);
            DrawingSystem.SetValue(Grid.ColumnProperty, 1);
            DrawingSystem.SetValue(Grid.ColumnSpanProperty, 2);

            MainGrid.Children.Add(DrawingSystem);

            BackGroundWorkerNonVisualization = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            BackGroundWorkerNonVisualization.DoWork += new DoWorkEventHandler(BackGroundWorkerNonVisualizationDoWork);
            BackGroundWorkerNonVisualization.ProgressChanged += new ProgressChangedEventHandler(BackGroundWorkerNonVisualizationProgressChanged);
            BackGroundWorkerNonVisualization.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackGroundWorkerNonVisualizationRunWorkerCompleted);

            ComboBoxTimeBase.SelectedIndex = 0;
            SimulationRunning = false;
            SimulationInitialized = false;

            SimulationTimer = new DispatcherTimer();
            SimulationTimer.Tick += PlaySimulationTick;
            TextBoxSimSpeed.Text = SimulationSpeed.ToString();

            CreateModel();

            SimulationDisplayClock = new DrawAnalogClockWithDate(Colors.Black, 0, 0)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            SimulationDisplayClock.SetValue(Grid.RowProperty, 0);
            SimulationDisplayClock.SetValue(Grid.ColumnProperty, 2);

            MainGrid.Children.Add(SimulationDisplayClock);

            Loaded += delegate
            {
                SimulationDisplayClock.Initialize(SimulationModel?.StartTime ?? DateTime.Now);
            };
        }

        public BackgroundWorker BackGroundWorkerNonVisualization { get; private set; }

        public TimeSpan ConstantTimerStep { get; set; }

        public DateTime CurrentTime { get; private set; }

        public DrawingOnCoordinateSystem DrawingSystem { get; }

        public DateTime NextSimulationTime { get; private set; }

        public DateTime NextTime =>
            NextTimerTime.Ticks < NextSimulationTime.Ticks
                    ? NextTimerTime
                    : NextSimulationTime;

        public DateTime NextTimerTime { get; private set; }

        public DrawAnalogClockWithDate SimulationDisplayClock { get; }

        public SimulationEngine SimulationEngine { get; set; } = new SimulationEngine();

        public bool SimulationInitialized { get; private set; }

        public SimulationModel? SimulationModel { get; set; }

        public bool SimulationRunning { get; private set; }

        public double SimulationSpeed
        {
            get => _simulationSpeed;
            set => _simulationSpeed = Math.Max(value, 1);
        }

        public DispatcherTimer SimulationTimer { get; }

        public bool TickCallIsSimulationCall => NextSimulationTime <= NextTimerTime;

        public DateTime TimerTime { get; }

        public bool VisualizationEnabled { get; set; }

        public void ActionsAfterFinishingSimulationRun()
        {
            ProgressBarSimulationProgress.Value = 100;
            ButtonPlaySimulation.Content = FindResource("Play");

            SimulationEngine.LoggingEngine.CreateSimulationResult();
            SimulationModel?.CreateSimulationResultsAfterStop();
            SimulationRunning = false;
            SimulationInitialized = false;
            SimulationModel = null;
        }

        public void CreateModel()
        {
            DrawingSystem.ClearSystem();

            SimulationModel = new ActorDemoModel((DateTime)DatePickerStartDate.SelectedDate!, (DateTime)DatePickerEndDate.SelectedDate!);

            SimulationEngine = new SimulationEngine
            {
                LoggingEngine = new BaseLoggingEngine(SimulationModel),
                CreateEventLog = true,
                SimulationModel = SimulationModel
            };
            SimulationModel?.Initialize(SimulationEngine);
            SimulationModel?.InitializeVisualization(DrawingSystem);
        }

        public void NonVisualizationLoop()
        {
            while (BackGroundWorkerNonVisualization.CancellationPending == false
                && !(SimulationModel?.StopSimulation(CurrentTime) == true))
            {
                bool modelRunning = SimulationEngine.RunSingleStepSimulationModel(CurrentTime, out DateTime newTime);

                if (modelRunning)
                {
                    BackGroundWorkerNonVisualization.ReportProgress(SimulationEngine.SimulationModel.GetSimulationProgress(newTime));
                    CurrentTime = newTime;
                }
            }
        }

        public void PlaySimulationTick(object? sender, EventArgs e)
        {
            // security call in case the timer ticks bfore
            // it disabled by stop simulation
            if (!VisualizationEnabled)
                return;

            DateTime timeOfCall;

            if (TickCallIsSimulationCall)
            {
                timeOfCall = CurrentTime;

                bool modelRunning = SimulationEngine.RunSingleStepSimulationModel(CurrentTime, out DateTime newTime);

                // in case end of model is reached the simulation is stopped
                if (!modelRunning)
                {
                    StopSimulation(false);
                    return;
                }

                SimulationDisplayClock.SetDateTime(CurrentTime);
                SimulationModel?.SimulationDrawingEngine.CreateModelVisualization(CurrentTime, SimulationModel, SimulationEngine.CurrentlyTriggeredEvents);
                ProgressBarSimulationProgress.Value = SimulationModel?.GetSimulationProgress(CurrentTime) ?? 0.0;

                NextSimulationTime = newTime;
                CurrentTime = newTime;
            }
            else
            {
                timeOfCall = NextTimerTime;

                SimulationDisplayClock.SetDateTime(NextTimerTime);
                SimulationModel?.SimulationDrawingEngine.CreateModelVisualization(NextTimerTime, SimulationModel, []);
                ProgressBarSimulationProgress.Value = SimulationModel?.GetSimulationProgress(CurrentTime) ?? 0.0;
                NextTimerTime += ConstantTimerStep;
            }

            SimulationTimer.Interval = TimeSpan.FromMilliseconds(Math.Max((int)(((NextTime - timeOfCall).Ticks / ConstantTimerStep.Ticks) * SimulationSpeed), 1));
        }

        public void RunSimulation(double firstWait = 0)
        {
            SimulationRunning = true;

            if (VisualizationEnabled)
            {
                SimulationTimer.Interval = TimeSpan.FromMilliseconds(firstWait);
                SimulationTimer.Start();
            }
            else
            {
                BackGroundWorkerNonVisualization.RunWorkerAsync();
            }
        }

        public void StopSimulation(bool pauseSim)
        {
            if (VisualizationEnabled)
            {
                SimulationTimer.Stop();
            }
            else
            {
                BackGroundWorkerNonVisualization.CancelAsync();
            }

            SimulationRunning = false;

            if (!pauseSim)
            {
                ActionsAfterFinishingSimulationRun();
            }
        }

        public TimeSpan TransformControlsToSimulationGap()
        {
            switch (ComboBoxTimeBase.SelectedIndex)
            {
                case 0:
                    return TimeSpan.FromSeconds(1);

                case 1:
                    return TimeSpan.FromMinutes(1);

                case 2:
                    return TimeSpan.FromHours(1);

                case 3:
                    return TimeSpan.FromDays(1);

                case 4:
                    return TimeSpan.FromDays(1);

                case 5:
                    return TimeSpan.FromDays(1);

                case 6:
                    return TimeSpan.FromDays(1);

                default:
                    break;
            }

            return TimeSpan.FromDays(1);
        }

        private void BackGroundWorkerNonVisualizationDoWork(object? sender, DoWorkEventArgs e)
        {
            NonVisualizationLoop();
        }

        private void BackGroundWorkerNonVisualizationProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            ProgressBarSimulationProgress.Value = e.ProgressPercentage;
        }

        private void BackGroundWorkerNonVisualizationRunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (SimulationModel?.StopSimulation(CurrentTime) == true)
            {
                ActionsAfterFinishingSimulationRun();
            }
        }

        private void MainWindowOnLoad(object sender, RoutedEventArgs e)
        {
            SimulationModel?.SimulationDrawingEngine.InitializeModelVisualizationAtTime(SimulationModel.StartTime, SimulationModel);
        }

        #region Control buttons

        public void ForwardSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Forwarding makes only sense when visualization is enabled
            // otherwise the simulation runs as fast as possible with
            // no delays
            if (!VisualizationEnabled || !SimulationRunning)
            {
                return;
            }

            // stop the current timer
            StopSimulation(true);

            // set next timer time to the simulation time plus the constant timer step
            // this will cause a simulation execution of the timer
            NextTimerTime = NextSimulationTime + ConstantTimerStep;

            // re-start the timer
            RunSimulation();
        }

        public void StopSimulation_Click(object sender, RoutedEventArgs e)
        {
            StopSimulation(false);
        }

        private void PlaySimulation_Click(object sender, RoutedEventArgs e)
        {
            // this is needed as pause and run are handled by the same button
            if (!SimulationInitialized)
            {
                if (SimulationModel == null)
                {
                    CreateModel();
                }

                // initialize visualization system
                SimulationModel!.InitializeVisualization(DrawingSystem);

                // initialize drawing engine
                SimulationModel.SimulationDrawingEngine.InitializeModelVisualizationAtTime(SimulationModel.StartTime, SimulationModel);

                // times are set
                CurrentTime = SimulationModel.StartTime;
                NextSimulationTime = SimulationModel.StartTime;
                NextTimerTime = SimulationModel.StartTime + ConstantTimerStep;

                SimulationInitialized = true;
            }

            SimulationTimer.Interval = TimeSpan.FromMilliseconds(SimulationSpeed);

            if (SimulationRunning)
            {
                ButtonPlaySimulation.Content = FindResource("Play");

                StopSimulation(true);
            }
            else
            {
                ButtonPlaySimulation.Content = FindResource("Pause");

                RunSimulation();
            }
        }

        #endregion Control buttons

        #region Other controls

        public void SimulationSpeedSlider_ValueChanged(object sender, EventArgs e)
        {
            double speedChosen = ((Slider)sender).Value;

            if (speedChosen <= 0)
            {
                TextBoxSimSpeed.Text = (32 * Math.Pow(2, -speedChosen)).ToString();
            }
            else
            {
                TextBoxSimSpeed.Text = (32 / Math.Pow(2d, speedChosen)).ToString();
            }

            SimulationSpeed = 32 / Math.Pow(2, speedChosen);

            if (SimulationRunning && VisualizationEnabled)
            {
                StopSimulation(true);

                RunSimulation(SimulationSpeed);
            }
        }

        private void CheckBoxShowVisualization_Checked(object sender, RoutedEventArgs e)
        {
            if (SimulationRunning)
            {
                StopSimulation(true);

                VisualizationEnabled = CheckBoxShowVisualization.IsChecked ?? false;

                if (VisualizationEnabled)
                {
                    NextTimerTime = CurrentTime + ConstantTimerStep;
                    // initialize drawing engine
                    SimulationModel?.SimulationDrawingEngine.InitializeModelVisualizationAtTime(CurrentTime, SimulationModel);
                }

                RunSimulation();
            }
            else
            {
                VisualizationEnabled = CheckBoxShowVisualization.IsChecked ?? false;
            }
        }

        private void ComboBoxTimeBaseSelectedIndexChanged(object sender, EventArgs e)
        {
            ConstantTimerStep = TransformControlsToSimulationGap();

            if (SimulationRunning && VisualizationEnabled)
            {
                StopSimulation(true);

                RunSimulation(SimulationSpeed);
            }
        }

        #endregion Other controls
    }
}