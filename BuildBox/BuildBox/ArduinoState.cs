namespace BuildBox
{
  public class ArduinoState
  {
    public BuildState BuildState { get; set; }

    public bool BrokenByMe { get; set; }

    public int Progress { get; set; }
  }
}