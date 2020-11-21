﻿//-----------------------------------------------------------------------
// <copyright file="DispatcherAdapter.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Threading;

namespace ThScoreFileConverter.Wrappers
{
    /// <summary>
    /// Wrapper of <see cref="Dispatcher"/>.
    /// </summary>
    internal class DispatcherAdapter : IDispatcherAdapter
    {
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherAdapter"/> class.
        /// </summary>
        /// <param name="dispatcher">
        /// <see cref="Dispatcher"/> to be wrapped;
        /// if <c>null</c>, <see cref="Application.Current"/>.Dispatcher is used.
        /// </param>
        public DispatcherAdapter(Dispatcher? dispatcher = null)
        {
            this.dispatcher = dispatcher ?? Application.Current.Dispatcher;
        }

        /// <inheritdoc/>
        public void Invoke(Action callback)
        {
            this.dispatcher.Invoke(callback);
        }
    }
}
