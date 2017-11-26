﻿namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using System;
    using Services;

    public interface IConfigurationSource
    {
        string GetConfigurationValueForKey(IService service, GraphContext graphContext, GraphNodeContext graphNodeContext, Type type);
    }
}