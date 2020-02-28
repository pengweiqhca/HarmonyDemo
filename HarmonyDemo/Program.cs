using HarmonyLib;
using System;
using System.Collections.Generic;

namespace HarmonyDemo
{
    [HarmonyPatch(typeof(Annotations))]
    [HarmonyPatch(nameof(Annotations.GetNumbers))]
    class Program
    {
        static void Main(string[] args)
        {
            var harmony = new Harmony(nameof(Program));

            harmony.PatchAll();

            Console.WriteLine(string.Join(", ", new Annotations().GetNumbers()));
            Console.WriteLine(string.Join(", ", new Annotations().GetNumbers()));
        }

        static void Prefix()
        {
            Console.WriteLine(nameof(Prefix));
        }
    }

    public class Annotations
    {
        public IEnumerable<int> GetNumbers()
        {
            Console.WriteLine(nameof(GetNumbers));

            yield return 1;
            yield return 2;
            yield return 3;
        }
    }
}
