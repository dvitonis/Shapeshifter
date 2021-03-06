﻿namespace Shapeshifter.WindowsDesktop.Mediators.Interfaces
{
    using System;

    using Infrastructure.Events;

    using Services.Interfaces;

    public interface IPasteCombinationDurationMediator: IHookService
    {
        event EventHandler<PasteCombinationDurationPassedEventArgument> PasteCombinationDurationPassed;

        event EventHandler<PasteCombinationReleasedEventArgument> PasteCombinationReleased;
        event EventHandler<PasteCombinationReleasedEventArgument> AfterPasteCombinationReleased;

        int DurationInDeciseconds { get; }

        bool IsCombinationFullyHeldDown { get; }

        bool IsCombinationPartiallyHeldDown { get; }

        void CancelCombinationRegistration();
    }
}