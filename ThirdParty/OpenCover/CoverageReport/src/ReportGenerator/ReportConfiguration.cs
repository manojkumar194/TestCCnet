﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Palmmedia.ReportGenerator.Common;
using Palmmedia.ReportGenerator.Properties;
using Palmmedia.ReportGenerator.Reporting.Rendering;

namespace Palmmedia.ReportGenerator
{
    /// <summary>
    /// Provides all parameters that are required for report generation.
    /// </summary>
    public class ReportConfiguration
    {
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReportConfiguration));

        /// <summary>
        /// The report files.
        /// </summary>
        private List<string> reportFiles = new List<string>();

        /// <summary>
        /// The report file pattern that could not be parsed.
        /// </summary>
        private List<string> failedReportFilePatterns = new List<string>();

        /// <summary>
        /// Determines whether the report type was successfully parsed during initialization.
        /// </summary>
        private bool reportTypeValid = true;

        /// <summary>
        /// Determines whether the verbosity level was successfully parsed during initialization.
        /// </summary>
        private bool verbosityLevelValid = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportConfiguration"/> class.
        /// </summary>
        /// <param name="reportFilePatterns">The report file patterns.</param>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="reportTypes">The report types.</param>
        /// <param name="sourceDirectories">The source directories.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="verbosityLevel">The verbosity level.</param>
        public ReportConfiguration(
            IEnumerable<string> reportFilePatterns,
            string targetDirectory,
            IEnumerable<string> reportTypes,
            IEnumerable<string> sourceDirectories,
            IEnumerable<string> filters,
            string verbosityLevel)
        {
            if (reportFilePatterns == null)
            {
                throw new ArgumentNullException("reportFilePatterns");
            }

            if (targetDirectory == null)
            {
                throw new ArgumentNullException("targetDirectory");
            }

            if (reportTypes == null)
            {
                throw new ArgumentNullException("reportTypes");
            }

            if (sourceDirectories == null)
            {
                throw new ArgumentNullException("sourceDirectories");
            }

            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }

            foreach (var reportFilePattern in reportFilePatterns)
            {
                try
                {
                    this.reportFiles.AddRange(FileSearch.GetFiles(reportFilePattern));
                }
                catch (Exception)
                {
                    this.failedReportFilePatterns.Add(reportFilePattern);
                }
            }

            this.TargetDirectory = targetDirectory;

            if (reportTypes.Any())
            {
                foreach (var reportType in reportTypes)
                {
                    ReportTypes parsedReportType = Reporting.Rendering.ReportTypes.Html;
                    this.reportTypeValid &= Enum.TryParse<ReportTypes>(reportType, true, out parsedReportType);
                    this.ReportType |= parsedReportType;
                }
            }
            else
            {
                this.ReportType = ReportTypes.Html;
            }

            this.SourceDirectories = sourceDirectories;
            this.Filters = filters;

            if (verbosityLevel != null)
            {
                VerbosityLevel parsedVerbosityLevel = VerbosityLevel.Verbose;
                this.verbosityLevelValid = Enum.TryParse<VerbosityLevel>(verbosityLevel, true, out parsedVerbosityLevel);
                this.VerbosityLevel = parsedVerbosityLevel;
            }
        }

        /// <summary>
        /// Gets the report files.
        /// </summary>
        public IEnumerable<string> ReportFiles
        { 
            get
            {
                return this.reportFiles;
            }
        }

        /// <summary>
        /// Gets the target directory.
        /// </summary>
        public string TargetDirectory { get; private set; }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        public ReportTypes ReportType { get; private set; }

        /// <summary>
        /// Gets the source directories.
        /// </summary>
        public IEnumerable<string> SourceDirectories { get; private set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public IEnumerable<string> Filters { get; private set; }

        /// <summary>
        /// Gets the verbosity level.
        /// </summary>
        public VerbosityLevel VerbosityLevel { get; private set; }

        /// <summary>
        /// Validates all parameters.
        /// </summary>
        /// <returns><c>true</c> if all parameters are in a valid state; otherwise <c>false</c>.</returns>
        internal bool Validate()
        {
            bool result = true;

            if (this.failedReportFilePatterns.Count > 0)
            {
                foreach (var failedReportFilePattern in this.failedReportFilePatterns)
                {
                    Logger.ErrorFormat(Resources.FailedReportFilePattern, failedReportFilePattern);
                }
                
                result &= false;
            }

            if (!this.ReportFiles.Any())
            {
                Logger.Error(Resources.NoReportFiles);
                result &= false;
            }
            else
            {
                foreach (var file in this.ReportFiles)
                {
                    if (!File.Exists(file))
                    {
                        Logger.ErrorFormat(Resources.NotExistingReportFile, file);
                        result &= false;
                    }
                }
            }

            if (string.IsNullOrEmpty(this.TargetDirectory))
            {
                Logger.Error(Resources.NoTargetDirectory);
                result &= false;
            }
            else if (!Directory.Exists(this.TargetDirectory))
            {
                try
                {
                    Directory.CreateDirectory(this.TargetDirectory);
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat(Resources.TargetDirectoryCouldNotBeCreated, this.TargetDirectory, ex.Message);
                    result &= false;
                }
            }

            if (!this.reportTypeValid)
            {
                Logger.Error(Resources.UnknownReportType);
                result &= false;
            }

            foreach (var directory in this.SourceDirectories)
            {
                if (!Directory.Exists(directory))
                {
                    Logger.ErrorFormat(Resources.SourceDirectoryDoesNotExist, directory);
                    result &= false;
                }
            }

            foreach (var filter in this.Filters)
            {
                if (string.IsNullOrEmpty(filter)
                    || (!filter.StartsWith("+", StringComparison.OrdinalIgnoreCase)
                        && !filter.StartsWith("-", StringComparison.OrdinalIgnoreCase)))
                {
                    Logger.ErrorFormat(Resources.InvalidFilter, filter);
                    result &= false;
                }
            }

            if (!this.verbosityLevelValid)
            {
                Logger.Error(Resources.UnknownVerbosityLevel);
                result &= false;
            }

            return result;
        }
    }
}
