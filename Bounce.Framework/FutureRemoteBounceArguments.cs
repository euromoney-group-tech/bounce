﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class FutureRemoteBounceArguments : TaskWithValue<string> {
        public object Targets { get; set; }
        public IEnumerable<IParameter> Parameters { get; set; }

        private ITargetsParser TargetsParser;
        private ILogOptionCommandLineTranslator LogOptionCommandLineTranslator;
        private readonly ICommandLineTasksParametersGenerator CommandLineTasksParametersGenerator;
        private string GeneratedBounceArguments;

        public FutureRemoteBounceArguments(ITargetsParser targetsParser, ILogOptionCommandLineTranslator logOptionCommandLineTranslator, ICommandLineTasksParametersGenerator commandLineTasksParametersGenerator) {
            TargetsParser = targetsParser;
            LogOptionCommandLineTranslator = logOptionCommandLineTranslator;
            CommandLineTasksParametersGenerator = commandLineTasksParametersGenerator;
            Parameters = new IParameter[0];
        }

        public FutureRemoteBounceArguments() : this(new TargetsParser(), new LogOptionCommandLineTranslator(), new CommandLineTasksParametersGenerator()) {}

        public override string Value
        {
            get { return GeneratedBounceArguments; }
        }

        public override void Invoke(BounceCommand command, IBounce bounce) {
            GeneratedBounceArguments = GetBounceArguments(bounce, command);
        }

        private string GetBounceArguments(IBounce bounce, BounceCommand command) {
            var args = new List<string>();

            args.Add(LogOptionCommandLineTranslator.GenerateCommandLine(bounce));

            IDictionary<string, ITask> targetsFromObject = TargetsParser.ParseTargetsFromObject(Targets);
            args.Add(command.ToString().ToLower());
            args.AddRange(targetsFromObject.Keys);
            args.Add(CommandLineTasksParametersGenerator.GenerateCommandLineParametersForTasks(targetsFromObject.Values, Parameters));

            return String.Join(" ", args.ToArray());
        }

        public FutureRemoteBounceArguments WithRemoteParameter<T>(IParameter parameter, T value) {
            return new FutureRemoteBounceArguments {Targets = Targets, Parameters = Parameters.Concat(new [] {new Parameter<T>(parameter.Name, value)})};
        }
    }
}