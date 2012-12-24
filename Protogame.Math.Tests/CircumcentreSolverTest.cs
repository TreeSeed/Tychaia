﻿using NUnit.Framework;
using System;

namespace Protogame.Math.Tests
{
    [TestFixture]
    public class CircumcentreSolverTest
    {
        private void AssertSphere(double[] a, double[] b, double[] c, double[] d, double[] centre, double radius)
        {
            CircumcentreSolver solver = new CircumcentreSolver(a, b, c, d);
            Assert.AreEqual(centre[0], solver.Centre[0]);
            Assert.AreEqual(centre[1], solver.Centre[1]);
            Assert.AreEqual(centre[2], solver.Centre[2]);
            Assert.AreEqual(radius, solver.Radius);
        }

        [Test]
        public void TestSolver()
        {
            this.AssertSphere(
                new double[] { 0, 1, 0 },
                new double[] { 0, -1, 0 },
                new double[] { 4, 0, 0 },
                new double[] { 0, 0, 1 },
                new double[] { 1.875, 0, 0 },
                2.125);
            this.AssertSphere(
                new double[] { 0, 1, 0 },
                new double[] { 0, -1, 0 },
                new double[] { 1, 0, 0 },
                new double[] { 0, 0, 1 },
                new double[] { 0, 0, 0 },
                1);
        }
    }
}
