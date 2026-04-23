using System;

namespace Droids.Model
{
    [Flags]
    public enum CollisionChecker
    {
        None = 0,
        Bottom = 1,
        Left = 2,
        Right = 4,
        Top = 8,
    }

    [Flags]
    public enum ButtonInput
    {
        None = 0,
        A = 1,
        B = 2,
        X = 4,
        Y = 8
    }

    [Flags]
    public enum SkillModifiers
    {
        None = 0,
        Grounded = 1,
        CanPassThroughPlatfornm = 2
    }

}
