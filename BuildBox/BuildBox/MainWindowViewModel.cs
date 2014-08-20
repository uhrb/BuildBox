using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using BuildBox.Annotations;
using BuildBox.TeamCity;

namespace BuildBox
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private Build _currentBuild;
    private string _currentComPort;
    private ObservableCollection<string> _portsAvailiable;
    private bool _comConnected;

    public MainWindowViewModel()
    {
      var lst = new List<string>(SerialPort.GetPortNames()) { string.Empty };
      PortsAvailiable = new ObservableCollection<string>();
      foreach (var s in lst)
      {
        PortsAvailiable.Add(s);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool ComConnected
    {
      get
      {
        return _comConnected;
      }

      set
      {
        if (value.Equals(_comConnected))
        {
          return;
        }

        _comConnected = value;
        OnPropertyChanged();
      }
    }

    public Build CurrentBuild
    {
      get
      {
        return _currentBuild;
      }

      set
      {
        if (Equals(value, _currentBuild))
        {
          return;
        }

        _currentBuild = value;
        OnPropertyChanged();
      }
    }

    public string CurrentComPort
    {
      get
      {
        return _currentComPort;
      }

      set
      {
        if (value == _currentComPort)
        {
          return;
        }

        _currentComPort = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<string> PortsAvailiable
    {
      get
      {
        return _portsAvailiable;
      }

      set
      {
        if (Equals(value, _portsAvailiable))
        {
          return;
        }

        _portsAvailiable = value;
        OnPropertyChanged();
      }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
