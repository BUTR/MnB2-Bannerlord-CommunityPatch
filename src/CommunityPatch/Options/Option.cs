namespace CommunityPatch {

  public class Option<TOption> where TOption : unmanaged {

    private readonly OptionsStore _options;

    public readonly string Namespace;

    public readonly string Name;

    public string QualifiedName
      => !string.IsNullOrEmpty(Namespace) ? $"{Namespace}.{Name}" : Name;

    public Option(OptionsStore options, string ns, string name) {
      _options = options;
      Namespace = ns;
      Name = name;
    }

    private TOption Value {
      get => _options.Get<TOption>(Namespace, Name);
      set => _options.Set(Namespace, Name, value);
    }

  }

}