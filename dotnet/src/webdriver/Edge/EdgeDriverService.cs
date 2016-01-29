// <copyright file="EdgeDriverService.cs" company="Microsoft">
// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership. The SFC licenses this file
// to you under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OpenQA.Selenium.Internal;

namespace OpenQA.Selenium.Edge
{
    /// <summary>
    /// Exposes the service provided by the native MicrosoftWebDriver executable.
    /// </summary>
    public sealed class EdgeDriverService : DriverService
    {
        private const string MicrosoftWebDriverServiceFileName = "MicrosoftWebDriver.exe";
        private static readonly Uri MicrosoftWebDriverDownloadUrl = new Uri("http://go.microsoft.com/fwlink/?LinkId=619687");
        private string applicationUserModelId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriverService"/> class.
        /// </summary>
        /// <param name="executablePath">The full path to the EdgeDriver executable.</param>
        /// <param name="executableFileName">The file name of the EdgeDriver executable.</param>
        /// <param name="port">The port on which the EdgeDriver executable should listen.</param>
        /// <param name="applicationUserModelId">The unique identifier for the WWA that EdgeDriver should launch</param>
        private EdgeDriverService(string executablePath, string executableFileName, int port, string applicationUserModelId = null)
            : base(executablePath, port, executableFileName, MicrosoftWebDriverDownloadUrl)
        {
            this.applicationUserModelId = applicationUserModelId;
        }

        /// <summary>
        /// Gets the command-line arguments for the driver service.
        /// </summary>
        protected override string CommandLineArguments
        {
            get
            {
                string wwaDriverServiceArgs = this.applicationUserModelId == null ?
                    string.Empty : string.Format(CultureInfo.InvariantCulture, "--package={0}", this.applicationUserModelId);

                return base.CommandLineArguments + " " + wwaDriverServiceArgs;
            }
        }

        /// <summary>
        /// Creates a default instance of the EdgeDriverService.
        /// </summary>
        /// <returns>A EdgeDriverService that implements default settings.</returns>
        public static EdgeDriverService CreateDefaultService()
        {
            string serviceDirectory = DriverService.FindDriverServiceExecutable(MicrosoftWebDriverServiceFileName, MicrosoftWebDriverDownloadUrl);
            return CreateDefaultService(serviceDirectory);
        }

        /// <summary>
        /// Creates a default instance of the EdgeDriverService using a specified path to the EdgeDriver executable.
        /// </summary>
        /// <param name="driverPath">The directory containing the EdgeDriver executable.</param>
        /// <returns>A EdgeDriverService using a random port.</returns>
        public static EdgeDriverService CreateDefaultService(string driverPath)
        {
            return CreateDefaultService(driverPath, MicrosoftWebDriverServiceFileName);
        }

        /// <summary>
        /// Creates a default instance of the EdgeDriverService using a specified path to the EdgeDriver executable with the given name.
        /// </summary>
        /// <param name="driverPath">The directory containing the EdgeDriver executable.</param>
        /// <param name="driverExecutableFileName">The name of the EdgeDriver executable file.</param>
        /// <returns>A EdgeDriverService using a random port.</returns>
        public static EdgeDriverService CreateDefaultService(string driverPath, string driverExecutableFileName)
        {
            return CreateDefaultService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
        }

        /// <summary>
        /// Creates a default instance of the EdgeDriverService using a specified path to the EdgeDriver executable with the given name and listening port.
        /// </summary>
        /// <param name="driverPath">The directory containing the EdgeDriver executable.</param>
        /// <param name="driverExecutableFileName">The name of the EdgeDriver executable file</param>
        /// <param name="port">The port number on which the driver will listen</param>
        /// <returns>A EdgeDriverService using the specified port.</returns>
        public static EdgeDriverService CreateDefaultService(string driverPath, string driverExecutableFileName, int port)
        {
            return new EdgeDriverService(driverPath, driverExecutableFileName, port);
        }

        /// <summary>
        /// Creates a WWA instance of the EdgeDriverService using a specified path to the EdgeDriver executable with the given name, listening port and the applicationUserModelId of the installed WWA that will be launched by EdgeDriver.
        /// </summary>
        /// <param name="driverPath">The directory containing the EdgeDriver executable.</param>
        /// <param name="driverExecutableFileName">The name of the EdgeDriver executable file</param>
        /// <param name="port">The port number on which the driver will listen</param>
        /// <param name="applicationUserModelId">The unique identifier for the WWA that EdgeDriver should launch</param>
        /// <returns>A EdgeDriverService using the specified port.</returns>
        public static EdgeDriverService CreateWWAService(string driverPath, string driverExecutableFileName, int port, string applicationUserModelId)
        {
            return new EdgeDriverService(driverPath, driverExecutableFileName, port, applicationUserModelId);
        }
    }
}
