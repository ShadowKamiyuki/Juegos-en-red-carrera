using UnityEngine;

public static class MovementMatrix
{
    private static Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 ToIso(this Vector3 input) => isometricMatrix.MultiplyPoint3x4(input);
}
