using System;
using JetBrains.Annotations;

namespace CommunityPatch {

  [PublicAPI]
  public class OptionNamespace {

    private readonly OptionsStore _options;

    public readonly string Namespace;

    public OptionNamespace([NotNull] OptionsStore options, [CanBeNull] string ns) {
      _options = options;
      Namespace = ns;
    }

    public Option<TOption> GetOption<TOption>([NotNull] string name) where TOption : unmanaged, IEquatable<TOption>
      => new Option<TOption>(_options, Namespace, name);

  }

}