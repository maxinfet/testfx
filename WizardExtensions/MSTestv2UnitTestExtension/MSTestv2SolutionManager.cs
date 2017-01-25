﻿//   Copyright (c) Microsoft Corporation. All rights reserved.

namespace MSTestv2UnitTestExtension
{
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.TestPlatform.TestGeneration;
    using Microsoft.VisualStudio.TestPlatform.TestGeneration.Data;
    using Microsoft.VisualStudio.TestPlatform.TestGeneration.Logging;
    using Microsoft.VisualStudio.TestPlatform.TestGeneration.Model;
    using NuGet.VisualStudio;
    using System;
    using VSLangProj80;
    using VSLangProj;

    /// <summary>
    /// A solution manager for MSTestv2 unit tests.
    /// </summary>
    public class MSTestv2SolutionManager : SolutionManagerBase
    {
        /// <summary>
        /// The service provider to use to get the interfaces required.
        /// </summary>
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MSTestv2SolutionManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider to use to get the interfaces required.</param>
        /// <param name="naming">The naming object used to decide how projects, classes and methods are named and created.</param>
        /// <param name="directory">The directory object to use for directory operations.</param>
        public MSTestv2SolutionManager(IServiceProvider serviceProvider, INaming naming, IDirectory directory) : base(serviceProvider, naming, directory)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Performs any preparatory tasks that have to be done after a new unit test project has been created.
        /// </summary>
        /// <param name="unitTestProject">The <see cref="Project"/> of the unit test project that has just been created.</param>
        /// <param name="sourceMethod">The <see cref="CodeFunction2"/> of the source method that is to be unit tested.</param>
        protected override void OnUnitTestProjectCreated(Project unitTestProject, CodeFunction2 sourceMethod)
        {
            if (unitTestProject == null)
            {
                throw new ArgumentNullException("unitTestProject");
            }

            TraceLogger.LogInfo("MSTestv2SolutionManager.OnUnitTestProjectCreated: Adding reference to MSTestv2 assemblies through nuget.");

            base.OnUnitTestProjectCreated(unitTestProject, sourceMethod);

            this.EnsureNuGetReference(unitTestProject, "MSTest.TestAdapter", "1.1.10-rc2");
            this.EnsureNuGetReference(unitTestProject, "MSTest.TestFramework", "1.0.8-rc2");

            VSProject2 vsp = unitTestProject.Object as VSProject2;
            if (vsp != null)
            {
                Reference reference = vsp.References.Find(GlobalConstants.MSTestAssemblyName);
                if (reference != null)
                {
                    TraceLogger.LogInfo("MSTestv2SolutionManager.OnUnitTestProjectCreated: Removing reference to {0}", reference.Name);
                    reference.Remove();
                }
            }
        }
    }
}
