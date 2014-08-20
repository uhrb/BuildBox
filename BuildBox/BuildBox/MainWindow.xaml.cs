using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildBox.TeamCity;

namespace BuildBox
{
  public partial class MainWindow
  {
    private readonly Thread _thread;
    private readonly TeamCityDataProvider _dataProvider;

    public MainWindow()
    {
      _dataProvider = new TeamCityDataProvider();
      _thread = new Thread(StatusQueryWorker);
      _thread.Start();
      BoxModel = new ArduinoModel();
      BoxModel.Connected += BoxModel_Connected;
      BoxModel.Disconnected += BoxModel_Disconnected;
      Model = new MainWindowViewModel();
      Model.PropertyChanged += Model_PropertyChanged;
      InitializeComponent();
    }

    public MainWindowViewModel Model { get; set; }

    private ArduinoModel BoxModel { get; set; }

    public void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "CurrentComPort":
          ChangePort(Model.CurrentComPort);
          break;
      }
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      _thread.Abort();
      _thread.Join(1000);
    }

    private void ChangePort(string newOne)
    {
      BoxModel.Disconnect();
      if (string.IsNullOrEmpty(newOne))
      {
        return;
      }

      BoxModel.Connect(newOne);
      LogMessage("Port changed to " + newOne);
    }

    private void BoxModel_Disconnected(object sender, EventArgs e)
    {
      Model.ComConnected = false;
      LogMessage("Com disconnected");
    }

    private void BoxModel_Connected(object sender, EventArgs e)
    {
      Model.ComConnected = true;
      LogMessage("Com connected");
    }

    private void LogMessage(string msg)
    {
      Dispatcher.Invoke(() =>
      {
        if (lsbLog.Items.Count > 100)
        {
          lsbLog.Items.Clear();
        }
        
        lsbLog.Items.Add(msg);
        lsbLog.ScrollIntoView(lsbLog.Items[lsbLog.Items.Count-1]);
      });
    }

    private void StatusQueryWorker()
    {
      while (true)
      {
        try
        {
          LogMessage("Querying about builds....");
          var task = Task.Run(() => _dataProvider.QueryForRunningBuilds());
          task.Wait();
          var builds = task.Result; 
          var interested = builds.Build.Where(l => l.BuildTypeId == "Plex_BuildAll").ToArray();
          LogMessage("Current our builds " + interested.Length);
          var minimum = interested.Length == 0 ? 0 : interested.Min(l => l.Number);
          var selected = interested.FirstOrDefault(l => l.Number == minimum);
          var state = new ArduinoState
          {
            BrokenByMe = false,
            BuildState =
              selected == null
                ? BuildState.NoBuilds
                : (selected.Status == "SUCCESS" ? BuildState.Green : BuildState.Red),
            Progress = (int)(selected == null ? 0 : selected.PercentageComplete)
          };
          LogMessage(string.Format("Build state {0} {1} {2}", state.BrokenByMe, state.BuildState, state.Progress));
          BoxModel.SetState(state);
          
        }
        catch (ThreadAbortException)
        {
          return;
        }
        catch
        {
          LogMessage("Exception was thrown by status query worker");
        }
        Thread.Sleep(new TimeSpan(0, 0, 5));
      }
    }
  }
}
