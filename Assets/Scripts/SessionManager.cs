using System.Threading;
using UnityEngine;

public static class SessionManager {
    private static int nextId = 0;
    public static int Next() => Interlocked.Increment(ref nextId);
}
