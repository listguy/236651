
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linter
{
    
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamingSyntacticAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "Rename violating symbol to fit policy";
        private const string MessageFormat = "Identifier name '{0}' does not conform to the policy";
        private const string Description = "Remove unnecessary methods.";
        private const string Category = "236651 Naming Syntactic Policy";
        private const string DiagnosticId = "CS236651";
        
        // REGEX Patterns//
        Regex  upperCamelCaseRegex= new Regex(@"\b([A-Z][a-z]*[0-9]*)+\b");
        
        Regex lowerCamelCaseRegex = new Regex(@"\b[a-z]+[0-9]*([A-Z][a-z]*[0-9]*)*\b");

        Regex snakeCaseRegex = new Regex(@"\b[A-Z]+(_[A-Z]+)*\b");

        internal static DiagnosticDescriptor NamingPolicyRule =
            new DiagnosticDescriptor(
                DiagnosticId,
                Title,
                MessageFormat,
                Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true
               );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(NamingPolicyRule);

        private ImmutableArray<SymbolKind> SymbolKinds =>
            ImmutableArray.Create<SymbolKind>(
                SymbolKind.Method,
                SymbolKind.NamedType,
                SymbolKind.Parameter,
                SymbolKind.Property,
                SymbolKind.Field,
                SymbolKind.Local);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(SymbolAction, SymbolKinds);
        }

        private void SymbolAction(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;

            if (symbol.Kind is SymbolKind.Method or SymbolKind.NamedType)
            {
                bool isComforting = upperCamelCaseRegex.IsMatch(symbol.Name);
                if (isComforting) 
                    return;
                
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        NamingPolicyRule,
                        symbol.Locations[0],
                        symbol.Name));
                return;
            }
            
            if (symbol.Kind is  SymbolKind.Parameter or SymbolKind.Property or SymbolKind.Field or SymbolKind.Local)
            {
                bool isComforting = upperCamelCaseRegex.IsMatch(symbol.Name);
                if (isComforting) 
                    return;
                
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        NamingPolicyRule,
                        symbol.Locations[0],
                        symbol.Name));
                return;
            }
            
            // INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
            //
            // // Find just those named type symbols that have members with the same name as the named type.
            // if (namedTypeSymbol.GetMembers(namedTypeSymbol.Name).Any())
            // {
            //     // For all such symbols, report a diagnostic.
            //     context.ReportDiagnostic(
            //         Diagnostic.Create(
            //             Rule,
            //             namedTypeSymbol.Locations[0],
            //             namedTypeSymbol.Name));
            // }
        }
        
        private static void SymbolAction1(CodeBlockAnalysisContext codeBlockContext)
        {
            
            // // We only care about method bodies.
            // // if (codeBlockContext.OwningSymbol.Kind != SymbolKind.Method)
            // // {
            // //     return;
            // // }
            //
            // // Report diagnostic for void non-virtual methods with empty method bodies.
            // // IMethodSymbol method = (IMethodSymbol)codeBlockContext.OwningSymbol;
            // // BlockSyntax block = (BlockSyntax)codeBlockContext.CodeBlock.ChildNodes().FirstOrDefault(n => n.Kind() == SyntaxKind.Block);
            // // if (method.ReturnsVoid && !method.IsVirtual && block != null && block.Statements.Count == 0)
            // // {
            //     SyntaxTree tree = block.SyntaxTree;
            //     Location location = method.Locations.First(l => tree.Equals(l.SourceTree));
            //     Diagnostic diagnostic = Diagnostic.Create(Rule, location, method.Name);
            //     codeBlockContext.ReportDiagnostic(diagnostic);
            // // }
        }
    }
}