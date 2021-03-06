﻿using System.Globalization;
using System.Threading;
using ApprovalTests;
using ApprovalTests.Namers;
using Fody;
using NUnit.Framework;

public class CultureResourceTests : BaseCosturaTest
{
    private static readonly TestResult testResult;

    static CultureResourceTests()
    {
        testResult = WeavingHelper.CreateIsolatedAssemblyCopy("ExeToProcess.exe",
            "<Costura />",
            new[]
            {
                "AssemblyToReference.dll",
                "de\\AssemblyToReference.resources.dll",
                "fr\\AssemblyToReference.resources.dll",
                "AssemblyToReferencePreEmbedded.dll",
                "ExeToReference.exe"
            }, "Culture");
    }

    [Test]
    public void UsingResource()
    {
        var culture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");
            var instance1 = testResult.GetInstance("ClassToTest");
            Assert.AreEqual("Salut", instance1.InternationalFoo());
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }

    [Test]
    public void TemplateHasCorrectSymbols()
    {
        var dataPoints = GetScenarioName();

        using (ApprovalResults.ForScenario(dataPoints))
        {
            var text = Ildasm.Decompile(TestResult.AssemblyPath, "Costura.AssemblyLoader");
            Approvals.Verify(text);
        }
    }

    public override TestResult TestResult => testResult;
}
