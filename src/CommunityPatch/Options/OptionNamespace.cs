namespace CommunityPatch {

  public class OptionNamespace {

    private readonly OptionsStore _options;

    public readonly string Namespace;

    public OptionNamespace(OptionsStore options, string ns) {
      _options = options;
      Namespace = ns;
    }

    public Option<T> GetOption<T>(string name) where T : unmanaged
      => new Option<T>(_options, Namespace, name);

  }

}