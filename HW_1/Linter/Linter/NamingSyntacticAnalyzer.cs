
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
            ImmutableArray.Create(
                SymbolKind.Method,
                SymbolKind.NamedType,
                SymbolKind.Parameter,
                SymbolKind.Property,
                SymbolKind.Field
            );
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(SymbolAction, SymbolKinds);
            context.RegisterSyntaxNodeAction(SyntaxNodeAction, SyntaxKind.LocalDeclarationStatement);
        }

        private void SyntaxNodeAction(SyntaxNodeAnalysisContext context)
        {
            
            var node = (LocalDeclarationStatementSyntax)context.Node;

            foreach (var localVar in node.Declaration.Variables)
            {
                string varName = localVar.Identifier.Text;
                bool isComforting = lowerCamelCaseRegex.IsMatch(varName);
                    if (isComforting) 
                        return;

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        NamingPolicyRule,
                        localVar.Identifier.GetLocation(),
                        varName));
                
            }
            
            
        }

        private void SymbolAction(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;

            if (symbol.Kind is (SymbolKind.Method or SymbolKind.NamedType))
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

            if (symbol.Kind is (SymbolKind.Parameter or SymbolKind.Property))
            {
                bool isComforting = lowerCamelCaseRegex.IsMatch(symbol.Name);
                if (isComforting)
                    return;

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        NamingPolicyRule,
                        symbol.Locations[0],
                        symbol.Name));
                return;
            }

            if (symbol.Kind == SymbolKind.Field)
            {
                bool isComforting;
                IFieldSymbol field = (IFieldSymbol)symbol;
                if (field.IsConst)
                {
                    isComforting = snakeCaseRegex.IsMatch(field.Name);
                }
                else
                {
                    isComforting = lowerCamelCaseRegex.IsMatch(symbol.Name);
                }

                if (isComforting)
                    return;

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        NamingPolicyRule,
                        symbol.Locations[0],
                        symbol.Name));
            }
            
        }
    }
}