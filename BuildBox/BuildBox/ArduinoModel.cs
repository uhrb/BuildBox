using System;
using System.IO.Ports;
using System.Text;

namespace BuildBox
{
  public class ArduinoModel : IDisposable
  {
    private SerialPort _currentPort;

    public event EventHandler Connected;
    public event EventHandler Disconnected;
    public event EventHandler<string> DataArived;

    public void Connect(string comName)
    {
      if (_currentPort != null && _currentPort.IsOpen)
      {
        return;
      }

      _currentPort = new SerialPort(comName, 9600);
      _currentPort.Open();
      if (Connected != null)
      {
        Connected(this, EventArgs.Empty);
      }
    }

    public void Disconnect(bool suppressErrors = false)
    {
      if (_currentPort == null)
      {
        return;
      }

      if (suppressErrors)
      {
        try
        {
          _currentPort.Close();
          _currentPort = null;
        }
          // ReSharper disable once EmptyGeneralCatchClause OK Here
        catch
        {
        }
      }
      else
      {
        _currentPort.Close();
        _currentPort = null;
      }

      if (Disconnected != null)
      {
        Disconnected(this, EventArgs.Empty);
      }
    }

    public void SetState(ArduinoState state)
    {
      var s = string.Empty;
      switch (state.BuildState)
      {
          case BuildState.Green:
          s = "1";
          break;
          case BuildState.NoBuilds:
          s = "2";
          break;
          case BuildState.Red:
          s = "3";
          break;
      }

      if (state.BrokenByMe)
      {
        s += "1";
      }
      else
      {
        s += "0";
      }

      if (state.Progress < 0)
      {
        state.Progress = 0;
      }

      if (state.Progress > 100)
      {
        state.Progress = 100;
      }

      s = (state.Progress * 11 / 100).ToString("D2")  + s;
      if (_currentPort == null || !_currentPort.IsOpen)
      {
        return;
      }

      try
      {
        if (_currentPort.BytesToRead != 0)
        {
          var buffer = new byte[_currentPort.BytesToRead];
          _currentPort.Read(buffer, 0, buffer.Length);
          if (DataArived != null)
          {
            DataArived(this, Encoding.ASCII.GetString(buffer));
          }
        }

        _currentPort.Write(Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(s)));
      }
      catch
      {
        Disconnect(true);
      }
    }

    public void Dispose()
    {
      if (_currentPort == null || !_currentPort.IsOpen)
      {
        return;
      }

      _currentPort.Close();
      _currentPort = null;
    }
  }
}