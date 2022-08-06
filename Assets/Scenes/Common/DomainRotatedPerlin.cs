/**
 * Domain-Rotated (planar-artifact-mitigated) Perlin noise, with the unrotated base endpoint left public.
 */

using System.Runtime.CompilerServices;

public static class DomainRotatedPerlin {
    private const long PRIME_X = 0x5205402B9270C86FL;
    private const long PRIME_Y = 0x598CD327003817B5L;
    private const long PRIME_Z = 0x5BCC226E9FA0BACBL;
    private const long HASH_MULTIPLIER = 0x53A3F72DEEC546F5L;

    private const double ROOT3OVER3 = 0.577350269189626;
    private const double ROTATE_3D_ORTHOGONALIZER = -0.21132486540518713;

    private const int N_GRADS_3D_EXPONENT = 8;
    private const int N_GRADS_3D = 1 << N_GRADS_3D_EXPONENT;

    private const double NORMALIZER_3D = 2.742445288166158;

    public static float Noise3_ImproveXY(long seed, double x, double y, double z) {
        // Let Z point up the main diagonal of the noise grid, so that XY slices are
        // moved as far out of alignment with the cube faces as possible.
        double xy = x + y;
        double s2 = xy * ROTATE_3D_ORTHOGONALIZER;
        double zz = z * ROOT3OVER3;
        double xr = x + s2 + zz;
        double yr = y + s2 + zz;
        double zr = xy * -ROOT3OVER3 + zz;

        return Noise3_UnrotatedBase(seed, xr, yr, zr);
    }

    public static float Noise3_ImproveXZ(long seed, double x, double y, double z) {
        // Let Y point up the main diagonal of the noise grid, so that XZ slices are
        // moved as far out of alignment with the cube faces as possible.
        double xz = x + z;
        double s2 = xz * ROTATE_3D_ORTHOGONALIZER;
        double yy = y * ROOT3OVER3;
        double xr = x + s2 + yy;
        double zr = z + s2 + yy;
        double yr = xz * -ROOT3OVER3 + yy;

        return Noise3_UnrotatedBase(seed, xr, yr, zr);
    }

    public static float Noise3_UnrotatedBase(long seed, double xr, double yr, double zr) {

        int xrb = FastFloor(xr), yrb = FastFloor(yr), zrb = FastFloor(zr);
        long xrbp = xrb * PRIME_X, yrbp = yrb * PRIME_Y, zrbp = zrb * PRIME_Z;
        float xri = (float)(xr - xrb), yri = (float)(yr - yrb), zri = (float)(zr - zrb);
        float g000 = Grad(seed, xrbp, yrbp, zrbp, xri, yri, zri);
        float g001 = Grad(seed, xrbp, yrbp, zrbp + PRIME_Z, xri, yri, zri - 1);
        float g010 = Grad(seed, xrbp, yrbp + PRIME_Y, zrbp, xri, yri - 1, zri);
        float g011 = Grad(seed, xrbp, yrbp + PRIME_Y, zrbp + PRIME_Z, xri, yri - 1, zri - 1);
        float g100 = Grad(seed, xrbp + PRIME_X, yrbp, zrbp, xri - 1, yri, zri);
        float g101 = Grad(seed, xrbp + PRIME_X, yrbp, zrbp + PRIME_Z, xri - 1, yri, zri - 1);
        float g110 = Grad(seed, xrbp + PRIME_X, yrbp + PRIME_Y, zrbp, xri - 1, yri - 1, zri);
        float g111 = Grad(seed, xrbp + PRIME_X, yrbp + PRIME_Y, zrbp + PRIME_Z, xri - 1, yri - 1, zri - 1);
        float fadeX = FadeCurve(xri);
        float fadeY = FadeCurve(yri);
        float fadeZ = FadeCurve(zri);
        float g00Z = (1 - fadeZ) * g000 + fadeZ * g001;
        float g01Z = (1 - fadeZ) * g010 + fadeZ * g011;
        float g10Z = (1 - fadeZ) * g100 + fadeZ * g101;
        float g11Z = (1 - fadeZ) * g110 + fadeZ * g111;
        float g0YZ = (1 - fadeY) * g00Z + fadeY * g01Z;
        float g1YZ = (1 - fadeY) * g10Z + fadeY * g11Z;
        float gXYZ = (1 - fadeX) * g0YZ + fadeX * g1YZ;

        return gXYZ;
    }


    /*
     * Utility
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Grad(long seed, long xrvp, long yrvp, long zrvp, float dx, float dy, float dz) {
        long hash = (seed ^ xrvp) ^ (yrvp ^ zrvp);
        hash *= HASH_MULTIPLIER;
        hash ^= hash >> (64 - N_GRADS_3D_EXPONENT + 2);
        int gi = (int)hash & ((N_GRADS_3D - 1) << 2);
        return GRADIENTS_3D[gi | 0] * dx + GRADIENTS_3D[gi | 1] * dy + GRADIENTS_3D[gi | 2] * dz;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FastFloor(double x) {
        int xi = (int)x;
        return x < xi ? xi - 1 : xi;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FadeCurve(float t) {
        return t * t * t * (10 + t * (-15 + t * 6));
    }

    /*
     * Gradients
     */

    private static readonly float[] GRADIENTS_3D;
    static DomainRotatedPerlin() {
        GRADIENTS_3D = new float[N_GRADS_3D * 4];
        float[] grad3 = {
             2.22474487139f,       2.22474487139f,      -1.0f,                 0.0f,
             2.22474487139f,       2.22474487139f,       1.0f,                 0.0f,
             3.0862664687972017f,  1.1721513422464978f,  0.0f,                 0.0f,
             1.1721513422464978f,  3.0862664687972017f,  0.0f,                 0.0f,
            -2.22474487139f,       2.22474487139f,      -1.0f,                 0.0f,
            -2.22474487139f,       2.22474487139f,       1.0f,                 0.0f,
            -1.1721513422464978f,  3.0862664687972017f,  0.0f,                 0.0f,
            -3.0862664687972017f,  1.1721513422464978f,  0.0f,                 0.0f,
            -1.0f,                -2.22474487139f,      -2.22474487139f,       0.0f,
             1.0f,                -2.22474487139f,      -2.22474487139f,       0.0f,
             0.0f,                -3.0862664687972017f, -1.1721513422464978f,  0.0f,
             0.0f,                -1.1721513422464978f, -3.0862664687972017f,  0.0f,
            -1.0f,                -2.22474487139f,       2.22474487139f,       0.0f,
             1.0f,                -2.22474487139f,       2.22474487139f,       0.0f,
             0.0f,                -1.1721513422464978f,  3.0862664687972017f,  0.0f,
             0.0f,                -3.0862664687972017f,  1.1721513422464978f,  0.0f,
            //--------------------------------------------------------------------//
            -2.22474487139f,      -2.22474487139f,      -1.0f,                 0.0f,
            -2.22474487139f,      -2.22474487139f,       1.0f,                 0.0f,
            -3.0862664687972017f, -1.1721513422464978f,  0.0f,                 0.0f,
            -1.1721513422464978f, -3.0862664687972017f,  0.0f,                 0.0f,
            -2.22474487139f,      -1.0f,                -2.22474487139f,       0.0f,
            -2.22474487139f,       1.0f,                -2.22474487139f,       0.0f,
            -1.1721513422464978f,  0.0f,                -3.0862664687972017f,  0.0f,
            -3.0862664687972017f,  0.0f,                -1.1721513422464978f,  0.0f,
            -2.22474487139f,      -1.0f,                 2.22474487139f,       0.0f,
            -2.22474487139f,       1.0f,                 2.22474487139f,       0.0f,
            -3.0862664687972017f,  0.0f,                 1.1721513422464978f,  0.0f,
            -1.1721513422464978f,  0.0f,                 3.0862664687972017f,  0.0f,
            -1.0f,                 2.22474487139f,      -2.22474487139f,       0.0f,
             1.0f,                 2.22474487139f,      -2.22474487139f,       0.0f,
             0.0f,                 1.1721513422464978f, -3.0862664687972017f,  0.0f,
             0.0f,                 3.0862664687972017f, -1.1721513422464978f,  0.0f,
            -1.0f,                 2.22474487139f,       2.22474487139f,       0.0f,
             1.0f,                 2.22474487139f,       2.22474487139f,       0.0f,
             0.0f,                 3.0862664687972017f,  1.1721513422464978f,  0.0f,
             0.0f,                 1.1721513422464978f,  3.0862664687972017f,  0.0f,
             2.22474487139f,      -2.22474487139f,      -1.0f,                 0.0f,
             2.22474487139f,      -2.22474487139f,       1.0f,                 0.0f,
             1.1721513422464978f, -3.0862664687972017f,  0.0f,                 0.0f,
             3.0862664687972017f, -1.1721513422464978f,  0.0f,                 0.0f,
             2.22474487139f,      -1.0f,                -2.22474487139f,       0.0f,
             2.22474487139f,       1.0f,                -2.22474487139f,       0.0f,
             3.0862664687972017f,  0.0f,                -1.1721513422464978f,  0.0f,
             1.1721513422464978f,  0.0f,                -3.0862664687972017f,  0.0f,
             2.22474487139f,      -1.0f,                 2.22474487139f,       0.0f,
             2.22474487139f,       1.0f,                 2.22474487139f,       0.0f,
             1.1721513422464978f,  0.0f,                 3.0862664687972017f,  0.0f,
             3.0862664687972017f,  0.0f,                 1.1721513422464978f,  0.0f,
        };
        for (int i = 0; i < grad3.Length; i++) {
            grad3[i] = (float)(grad3[i] / NORMALIZER_3D);
        }
        for (int i = 0, j = 0; i < GRADIENTS_3D.Length; i++, j++) {
            if (j == grad3.Length) j = 0;
            GRADIENTS_3D[i] = grad3[j];
        }
    }
}
