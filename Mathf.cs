
namespace Leo.ECS
{
	public static class Mathf
	{
		public const float Epsilon = 0.00001f;
		public const float Deg2Rad = 0.0174532924f;
		public const float Rad2Deg = 57.29578f;

		public static float Max(float a, float b) => (a > b) ? a : b;
		public static float Min(float a, float b) => (a < b) ? a : b;
		public static float Abs(float val) => (val < 0) ? val * -1 : val;
		public static float Clamp(float val, float min, float max) => (val < min) ? min : (val > max) ? max : val;
		public static int Mod(int x, int m) => (x % m + m) % m;
		public static float Mod(float x, float m)
		{
			float r = x % m;
			return (r < 0) ? r + m : r;
		}
	}
}
