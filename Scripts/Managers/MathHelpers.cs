using UnityEngine;

public static class MathHelpers
{
    public static float ClampZero(ref this float value) => value = Mathf.Max(0, value);
    public static int ClampZero(ref this int value) => value = Mathf.Max(0, value);

    public static float EaseNearZero(float value) => value * Mathf.Min(1, Mathf.Abs(value));

    public static float ZeroSign(float value) => value == 0 ? 0 : Mathf.Sign(value);
    public static int ZeroSign(int value) => (int)(value == 0 ? 0 : Mathf.Sign(value));

    public static bool IntToBool(this int value) => value != 0;
    public static int BoolToInt(this bool value) => value ? 1 : 0;

    public static Vector2 PlanePosition(this Vector3 pos) => new Vector2(pos.x, pos.z);

    public static bool CoinFlip() => Random.Range(0, 2) == 0;

    public static string RandomString(params string[] names) => names[Random.Range(0, names.Length - 1)];
}