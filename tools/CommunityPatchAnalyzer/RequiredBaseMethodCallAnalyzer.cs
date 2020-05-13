using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityPatchAnalyzer {

  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class RequiredBaseMethodCallAnalyzer : DiagnosticAnalyzer {

    private static readonly DiagnosticDescriptor Rule
      = new DiagnosticDescriptor("RequireBaseMethodCall",
        "Base Method Call Required",
        "{0} is required to call it's base implementation.\n{1}",
        "Usage",
        DiagnosticSeverity.Error,
        true,
        "Call to virtual method's base is required.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
      = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context) {
      context.RegisterCompilationStartAction(AnalyzeMethodForBaseCall);
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
    }

    private static void AnalyzeMethodForBaseCall(CompilationStartAnalysisContext compilationStartContext)
      => compilationStartContext.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context) {
      if (!(context.Node is MethodDeclarationSyntax mds))
        return;

      if (!(ModelExtensions.GetDeclaredSymbol(context.SemanticModel, mds) is IMethodSymbol methodSymbol))
        return;

      if (!methodSymbol.IsOverride)
        return;

      if (methodSymbol.OverriddenMethod == null)
        return;

      var baseMethodSymbol = methodSymbol.OverriddenMethod;
      var attrs = baseMethodSymbol.GetAttributes();
      if (!attrs.Any(ad => string.Equals(
        ad.AttributeClass.MetadataName,
        nameof(RequireBaseMethodCallAttribute),
        StringComparison.OrdinalIgnoreCase)))
        return;

      var baseMethodName = baseMethodSymbol.Name;
      var methodName = baseMethodName;

      var invocations = mds.DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();

      var diagnostics = new StringBuilder();
      diagnostics.AppendLine($"Looking for invocation: {methodSymbol}");

      foreach (var inv in invocations) {
        var invSymInfo = ModelExtensions.GetSymbolInfo(context.SemanticModel, inv);
        if (SymbolEqualityComparer.Default.Equals(invSymInfo.Symbol, baseMethodSymbol))
          return;

        diagnostics.AppendLine($"Skipped call to {invSymInfo.Symbol!.ToDisplayString()}");
      }

      var diag = Diagnostic.Create(Rule, mds.GetLocation(), methodName, diagnostics.ToString());
      context.ReportDiagnostic(diag);
    }

  }

}