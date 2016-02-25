using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JustAssembly.DiffAlgorithm.Algorithm
{
    /// <summary>
    /// This Class implements the Difference Algorithm published in
    /// "An O(ND) Difference Algorithm and its Variations" by Eugene Myers
    /// Algorithmica Vol. 1 No. 2, 1986, p 251.  
    /// 
    /// There are many C, Java, Lisp implementations public available but they all seem to come
    /// from the same source (diffutils) that is under the (unfree) GNU public License
    /// and cannot be reused as a sourcecode for a commercial application.
    /// There are very old C implementations that use other (worse) algorithms.
    /// Microsoft also published sourcecode of a diff-tool (windiff) that uses some tree data.
    /// Also, a direct transfer from a C source to C# is not easy because there is a lot of pointer
    /// arithmetic in the typical C solutions and i need a managed solution.
    /// These are the reasons why I implemented the original published algorithm from the scratch and
    /// make it avaliable without the GNU license limitations.
    /// I do not need a high performance diff tool because it is used only sometimes.
    /// I will do some performace tweaking when needed.
    /// 
    /// The algorithm itself is comparing 2 arrays of numbers so when comparing 2 text documents
    /// each line is converted into a (hash) number. See DiffText(). 
    /// 
    /// Some chages to the original algorithm:
    /// The original algorithm was described using a recursive approach and comparing zero indexed arrays.
    /// Extracting sub-arrays and rejoining them is very performance and memory intensive so the same
    /// (readonly) data arrays are passed arround together with their lower and upper bounds.
    /// This circumstance makes the Lcs and Sms functions more complicate.
    /// I added some code to the Lcs function to get a fast response on sub-arrays that are identical,
    /// completely deleted or inserted.
    /// 
    /// The result from a comparisation is stored in 2 arrays that flag for modified (deleted or inserted)
    /// lines in the 2 data arrays. These bits are then analysed to produce a array of Item objects.
    /// 
    /// Further possible optimizations:
    /// (first rule: don't do it; second: don't do it yet)
    /// The arrays DataA and DataB are passed as parameters, but are never changed after the creation
    /// so they can be members of the class to avoid the paramter overhead.
    /// In Sms is a lot of boundary arithmetic in the for-D and for-k loops that can be done by increment
    /// and decrement of local variables.
    /// The DownVector and UpVector arrays are alywas created and destroyed each time the Sms gets called.
    /// It is possible to reuse tehm when transfering them to members of the class.
    /// See TODO: hints.
    /// 
    /// diff.cs: A port of the algorythm to C#
    /// Created by Matthias Hertel, see http://www.mathertel.de
    /// This work is licensed under a Creative Commons Attribution 2.0 Germany License.
    /// see http://creativecommons.org/licenses/by/2.0/de/
    /// 
    /// Changes:
    /// 2002.09.20 There was a "hang" in some situations.
    /// Now I undestand a little bit more of the Sms algorithm. 
    /// There have been overlapping boxes; that where analyzed partial differently.
    /// One return-point is enough.
    /// A assertion was added in CreateDiffs when in debug-mode, that counts the number of equal (no modified) lines in both arrays.
    /// They must be identical.
    /// 
    /// 2003.02.07 Out of bounds error in the Up/Down vector arrays in some situations.
    /// The two vetors are now accessed using different offsets that are adjusted using the start k-Line. 
    /// A test case is added. 
    /// 
    /// 2006.03.05 Some documentation and a direct Diff entry point.
    /// 
    /// 2006.03.08 Refactored the API to static methods on the Diff class to make usage simpler.
    /// 2006.03.10 using the standard Debug class for self-test now.
    ///            compile with: csc /target:exe /out:diffTest.exe /d:DEBUG /d:TRACE /d:SELFTEST Diff.cs
    /// </summary>
    public class MyersDiff : IDiffAlgorithm
    {
        private static readonly Lazy<MyersDiff> instance
            = new Lazy<MyersDiff>();

        public static MyersDiff Instance
        {
            get
            {
                return instance.Value;
            }
        }

        /// <summary>
        /// Shortest Middle Snake Return Data
        /// </summary>
        private struct Smsrd
        {
            internal int x, y;
            // internal int u, v;  // 2002.09.20: no need for 2 points 
        }

        #region self-Test

#if (SELFTEST)
		/// <summary>
		/// start a self- / box-test for some diff cases and report to the debug output.
		/// </summary>
		/// <param name="args">not used</param>
		/// <returns>always 0</returns>
		public static int Main(string[] args) {
		StringBuilder ret = new StringBuilder();
		string a, b;

		System.Diagnostics.ConsoleTraceListener ctl = new System.Diagnostics.ConsoleTraceListener(false);
		System.Diagnostics.Debug.Listeners.Add(ctl);

		System.Console.WriteLine("Diff Self Test...");
	  
		// test all changes
		a = "a,b,c,d,e,f,g,h,i,j,k,l".Replace(',', '\n');
		b = "0,1,2,3,4,5,6,7,8,9".Replace(',', '\n');
		System.Diagnostics.Debug.Assert(TestHelper(Diff.DiffText(a, b, false, false, false))
		== "12.10.0.0*", 
		"all-changes test failed.");
		System.Diagnostics.Debug.WriteLine("all-changes test passed.");
		// test all same
		a = "a,b,c,d,e,f,g,h,i,j,k,l".Replace(',', '\n');
		b = a;
		System.Diagnostics.Debug.Assert(TestHelper(Diff.DiffText(a, b, false, false, false))
		== "",
		"all-same test failed.");
		System.Diagnostics.Debug.WriteLine("all-same test passed.");

		// test snake
		a = "a,b,c,d,e,f".Replace(',', '\n');
		b = "b,c,d,e,f,x".Replace(',', '\n');
		System.Diagnostics.Debug.Assert(TestHelper(Diff.DiffText(a, b, false, false, false))
		== "1.0.0.0*0.1.6.5*",
		"snake test failed.");
		System.Diagnostics.Debug.WriteLine("snake test passed.");

		// 2002.09.20 - repro
		a = "c1,a,c2,b,c,d,e,g,h,i,j,c3,k,l".Replace(',', '\n');
		b = "C1,a,C2,b,c,d,e,I1,e,g,h,i,j,C3,k,I2,l".Replace(',', '\n');
		System.Diagnostics.Debug.Assert(TestHelper(Diff.DiffText(a, b, false, false, false))
		== "1.1.0.0*1.1.2.2*0.2.7.7*1.1.11.13*0.1.13.15*",
		"repro20020920 test failed.");
		System.Diagnostics.Debug.WriteLine("repro20020920 test passed.");
	  
		// 2003.02.07 - repro
		a = "F".Replace(',', '\n');
		b = "0,F,1,2,3,4,5,6,7".Replace(',', '\n');
		System.Diagnostics.Debug.Assert(TestHelper(Diff.DiffText(a, b, false, false, false))
		== "0.1.0.0*0.7.1.2*", 
		"repro20030207 test failed.");
		System.Diagnostics.Debug.WriteLine("repro20030207 test passed.");
	  
		// Muegel - repro
		a = "HELLO\nWORLD";
		b = "\n\nhello\n\n\n\nworld\n";
		System.Diagnostics.Debug.Assert(TestHelper(Diff.DiffText(a, b, false, false, false))
		== "2.8.0.0*", 
		"repro20030409 test failed.");
		System.Diagnostics.Debug.WriteLine("repro20030409 test passed.");

		// test some differences
		a = "a,b,-,c,d,e,f,f".Replace(',', '\n');
		b = "a,b,x,c,e,f".Replace(',', '\n');
		System.Diagnostics.Debug.Assert(TestHelper(Diff.DiffText(a, b, false, false, false))
		== "1.1.2.2*1.0.4.4*1.0.6.5*", 
		"some-changes test failed.");
		System.Diagnostics.Debug.WriteLine("some-changes test passed.");

		System.Diagnostics.Debug.WriteLine("End.");
		System.Diagnostics.Debug.Flush();

		return (0);
		}

		public static string TestHelper(Item []f) {
		StringBuilder ret = new StringBuilder();
		for (int n = 0; n < f.Length; n++) {
		ret.Append(f[n].deletedA.ToString() + "." + f[n].insertedB.ToString() + "." + f[n].StartA.ToString() + "." + f[n].StartB.ToString() + "*");
		}
		// Debug.Write(5, "TestHelper", ret.ToString());
		return (ret.ToString());
		}
#endif
        #endregion

        /// <summary>
        /// Find the difference in 2 texts, comparing by textlines.
        /// </summary>
        /// <param name="TextA">A-version of the text (usualy the old one)</param>
        /// <param name="TextB">B-version of the text (usualy the new one)</param>
        /// <returns>Returns a array of Items that describe the differences.</returns>
        public IList<DiffItem> DiffText(IList<string> textA, IList<string> textB)
        {
            return DiffText(textA, textB, false, false, false);
        }// DiffText

        /// <summary>
        /// Find the difference in 2 text documents, comparing by textlines.
        /// The algorithm itself is comparing 2 arrays of numbers so when comparing 2 text documents
        /// each line is converted into a (hash) number. This hash-value is computed by storing all
        /// textlines into a common hashtable so i can find dublicates in there, and generating a 
        /// new number each time a new textline is inserted.
        /// </summary>
        /// <param name="TextA">A-version of the text (usualy the old one)</param>
        /// <param name="TextB">B-version of the text (usualy the new one)</param>
        /// <param name="trimSpace">When set to true, all leading and trailing whitespace characters are stripped out before the comparation is done.</param>
        /// <param name="ignoreSpace">When set to true, all whitespace characters are converted to a single space character before the comparation is done.</param>
        /// <param name="ignoreCase">When set to true, all characters are converted to their lowercase equivivalence before the comparation is done.</param>
        /// <returns>Returns a array of Items that describe the differences.</returns>
        public static IList<DiffItem> DiffText(IList<string> textA, IList<string> textB, bool trimSpace, bool ignoreSpace, bool ignoreCase)
        {
            // prepare the input-text and convert to comparable numbers.
            var h = new Dictionary<string, int>(textA.Count + textB.Count);

            // The A-Version of the data (original data) to be compared.
            var dataA = new DiffData(DiffCodes(textA, h, trimSpace, ignoreSpace, ignoreCase));

            // The B-Version of the data (modified data) to be compared.
            var dataB = new DiffData(DiffCodes(textB, h, trimSpace, ignoreSpace, ignoreCase));

            h = null; // free up hashtable memory (maybe)

            int max = dataA.Length + dataB.Length + 1;
            int[] downVector = new int[2 * max + 2];
            int[] upVector = new int[2 * max + 2];

            Lcs(dataA, 0, dataA.Length, dataB, 0, dataB.Length, downVector, upVector);
            return CreateDiffs(dataA, dataB);
        }// DiffText

        /// <summary>
        /// Find the difference in 2 arrays of integers.
        /// </summary>
        /// <param name="ArrayA">A-version of the numbers (usualy the old one)</param>
        /// <param name="ArrayB">B-version of the numbers (usualy the new one)</param>
        /// <returns>Returns a array of Items that describe the differences.</returns>
        public static IList<DiffItem> DiffInt(int[] arrayA, int[] arrayB)
        {
            // The A-Version of the data (original data) to be compared.
            var dataA = new DiffData(arrayA);

            // The B-Version of the data (modified data) to be compared.
            var dataB = new DiffData(arrayB);

            int max = dataA.Length + dataB.Length + 1;
            int[] downVector = new int[2 * max + 2];
            int[] upVector = new int[2 * max + 2];

            Lcs(dataA, 0, dataA.Length, dataB, 0, dataB.Length, downVector, upVector);
            return CreateDiffs(dataA, dataB);
        }// Diff

        /// <summary>
        /// This function converts all textlines of the text into unique numbers for every unique textline
        /// so further work can work only with simple numbers.
        /// </summary>
        /// <param name="aText">the input text</param>
        /// <param name="h">This extern initialized hashtable is used for storing all ever used textlines.</param>
        /// <param name="trimSpace">ignore leading and trailing space characters</param>
        /// <returns>a array of integers.</returns>
        private static int[] DiffCodes(IList<string> lines, IDictionary h, bool trimSpace, bool ignoreSpace, bool ignoreCase)
        {
            // get all codes of the text
            int[] codes;
            int lastUsedCode = h.Count;
            object aCode;
            string s;

            codes = new int[lines.Count];

            for (int i = 0; i < lines.Count; ++i)
            {
                s = lines[i];

                // Commented to fix the modified last row when inserting new rows after. Adds " " and breaks the result by adding 1 modified before inserted rows
                //if (i == lines.Count - 1)
                //    s += " "; // Never produce a delete last lines change

                if (trimSpace)
                    s = s.Trim();

                if (ignoreSpace)
                {
                    s = Regex.Replace(s, "\\s+", " ");            // TODO: optimization: faster blank removal.
                }

                if (ignoreCase)
                    s = s.ToLower();

                aCode = h[s];
                if (aCode == null)
                {
                    lastUsedCode++;
                    h[s] = lastUsedCode;
                    codes[i] = lastUsedCode;
                }
                else
                {
                    codes[i] = (int)aCode;
                }// if
            }// for
            return (codes);
        }// DiffCodes

        /// <summary>
        /// This is the algorithm to find the Shortest Middle Snake (Sms).
        /// </summary>
        /// <param name="DataA">sequence A</param>
        /// <param name="LowerA">lower bound of the actual range in DataA</param>
        /// <param name="UpperA">upper bound of the actual range in DataA (exclusive)</param>
        /// <param name="DataB">sequence B</param>
        /// <param name="LowerB">lower bound of the actual range in DataB</param>
        /// <param name="UpperB">upper bound of the actual range in DataB (exclusive)</param>
        /// <returns>a MiddleSnakeData record containing x,y and u,v</returns>
        private static Smsrd Sms(DiffData dataA, int lowerA, int upperA, DiffData dataB, int lowerB, int upperB, int[] downVector, int[] upVector)
        {
            Smsrd ret;
            int max = dataA.Length + dataB.Length + 1;

            int downK = lowerA - lowerB; // the k-line to start the forward search
            int upK = upperA - upperB; // the k-line to start the reverse search

            int delta = (upperA - lowerA) - (upperB - lowerB);
            bool oddDelta = (delta & 1) != 0;

            /// vector for the (0,0) to (x,y) search
            Array.Clear(downVector, 0, downVector.Length);

            /// vector for the (u,v) to (N,M) search
            Array.Clear(upVector, 0, upVector.Length);

            // The vectors in the publication accepts negative indexes. the vectors implemented here are 0-based
            // and are access using a specific offset: UpOffset UpVector and DownOffset for DownVektor
            int downOffset = max - downK;
            int upOffset = max - upK;

            int maxD = ((upperA - lowerA + upperB - lowerB) / 2) + 1;

            // Debug.Write(2, "Sms", String.Format("Search the box: A[{0}-{1}] to B[{2}-{3}]", LowerA, UpperA, LowerB, UpperB));

            // init vectors
            downVector[downOffset + downK + 1] = lowerA;
            upVector[upOffset + upK - 1] = upperA;

            for (int D = 0; D <= maxD; D++)
            {
                // Extend the forward path.
                for (int k = downK - D; k <= downK + D; k += 2)
                {
                    // Debug.Write(0, "Sms", "extend forward path " + k.ToString());
                    // find the only or better starting point
                    int x, y;
                    if (k == downK - D)
                    {
                        x = downVector[downOffset + k + 1]; // down
                    }
                    else
                    {
                        x = downVector[downOffset + k - 1] + 1; // a step to the right
                        if ((k < downK + D) && (downVector[downOffset + k + 1] >= x))
                            x = downVector[downOffset + k + 1]; // down
                    }
                    y = x - k;

                    // find the end of the furthest reaching forward D-path in diagonal k.
                    while ((x < upperA) && (y < upperB) && (dataA.Data[x] == dataB.Data[y]))
                    {
                        x++;
                        y++;
                    }
                    downVector[downOffset + k] = x;

                    // overlap ?
                    if (oddDelta && (upK - D < k) && (k < upK + D))
                    {
                        if (upVector[upOffset + k] <= downVector[downOffset + k])
                        {
                            ret.x = downVector[downOffset + k];
                            ret.y = downVector[downOffset + k] - k;
                            // ret.u = UpVector[UpOffset + k];      // 2002.09.20: no need for 2 points 
                            // ret.v = UpVector[UpOffset + k] - k;
                            return (ret);
                        }// if
                    }// if
                }// for k

                // Extend the reverse path.
                for (int k = upK - D; k <= upK + D; k += 2)
                {
                    // Debug.Write(0, "Sms", "extend reverse path " + k.ToString());
                    // find the only or better starting point
                    int x, y;
                    if (k == upK + D)
                    {
                        x = upVector[upOffset + k - 1]; // up
                    }
                    else
                    {
                        x = upVector[upOffset + k + 1] - 1; // left
                        if ((k > upK - D) && (upVector[upOffset + k - 1] < x))
                            x = upVector[upOffset + k - 1]; // up
                    }// if
                    y = x - k;

                    while ((x > lowerA) && (y > lowerB) && (dataA.Data[x - 1] == dataB.Data[y - 1]))
                    {
                        x--;
                        y--; // diagonal
                    }
                    upVector[upOffset + k] = x;

                    // overlap ?
                    if (!oddDelta && (downK - D <= k) && (k <= downK + D))
                    {
                        if (upVector[upOffset + k] <= downVector[downOffset + k])
                        {
                            ret.x = downVector[downOffset + k];
                            ret.y = downVector[downOffset + k] - k;
                            // ret.u = UpVector[UpOffset + k];     // 2002.09.20: no need for 2 points 
                            // ret.v = UpVector[UpOffset + k] - k;
                            return (ret);
                        }// if
                    }// if
                }// for k
            }// for D

            throw new NotSupportedException("the algorithm should never come here.");
        }// Sms

        /// <summary>
        /// This is the divide-and-conquer implementation of the longes common-subsequence (Lcs) 
        /// algorithm.
        /// The published algorithm passes recursively parts of the A and B sequences.
        /// To avoid copying these arrays the lower and upper bounds are passed while the sequences stay constant.
        /// </summary>
        /// <param name="DataA">sequence A</param>
        /// <param name="LowerA">lower bound of the actual range in DataA</param>
        /// <param name="UpperA">upper bound of the actual range in DataA (exclusive)</param>
        /// <param name="DataB">sequence B</param>
        /// <param name="LowerB">lower bound of the actual range in DataB</param>
        /// <param name="UpperB">upper bound of the actual range in DataB (exclusive)</param>
        private static void Lcs(DiffData dataA, int lowerA, int upperA, DiffData dataB, int lowerB, int upperB, int[] downVector, int[] UpVector)
        {
            // Debug.Write(2, "Lcs", String.Format("Analyse the box: A[{0}-{1}] to B[{2}-{3}]", LowerA, UpperA, LowerB, UpperB));
            // Fast walkthrough equal lines at the start
            while (lowerA < upperA && lowerB < upperB && dataA.Data[lowerA] == dataB.Data[lowerB])
            {
                lowerA++;
                lowerB++;
            }

            // Fast walkthrough equal lines at the end
            while (lowerA < upperA && lowerB < upperB && dataA.Data[upperA - 1] == dataB.Data[upperB - 1])
            {
                --upperA;
                --upperB;
            }

            if (lowerA == upperA)
            {
                // mark as inserted lines.
                while (lowerB < upperB)
                    dataB.Modified[lowerB++] = true;
            }
            else if (lowerB == upperB)
            {
                // mark as deleted lines.
                while (lowerA < upperA)
                    dataA.Modified[lowerA++] = true;
            }
            else
            {
                // Find the middle snakea and length of an optimal path for A and B
                Smsrd smsrd = Sms(dataA, lowerA, upperA, dataB, lowerB, upperB, downVector, UpVector);
                // Debug.Write(2, "MiddleSnakeData", String.Format("{0},{1}", smsrd.x, smsrd.y));

                // The path is from LowerX to (x,y) and (x,y) ot UpperX
                Lcs(dataA, lowerA, smsrd.x, dataB, lowerB, smsrd.y, downVector, UpVector);
                Lcs(dataA, smsrd.x, upperA, dataB, smsrd.y, upperB, downVector, UpVector);  // 2002.09.20: no need for 2 points 
            }
        }// Lcs()

        /// <summary>Scan the tables of which lines are inserted and deleted,
        /// producing an edit script in forward order.  
        /// </summary>
        /// dynamic array
        private static IList<DiffItem> CreateDiffs(DiffData dataA, DiffData dataB)
        {
            var a = new List<DiffItem>();
            DiffItem aItem;
            DiffItem[] result;

            int startA, startB;
            int lineA, lineB;

            lineA = 0;
            lineB = 0;
            while (lineA < dataA.Length || lineB < dataB.Length)
            {
                if ((lineA < dataA.Length) && (!dataA.Modified[lineA]) && (lineB < dataB.Length) && (!dataB.Modified[lineB]))
                {
                    // equal lines
                    lineA++;
                    lineB++;
                }
                else
                {
                    // maybe deleted and/or inserted lines
                    startA = lineA;
                    startB = lineB;

                    while (lineA < dataA.Length && (lineB >= dataB.Length || dataA.Modified[lineA]))
                        // while (LineA < DataA.Length && DataA.modified[LineA])
                        lineA++;

                    while (lineB < dataB.Length && (lineA >= dataA.Length || dataB.Modified[lineB]))
                        // while (LineB < DataB.Length && DataB.modified[LineB])
                        lineB++;

                    if ((startA < lineA) || (startB < lineB))
                    {
                        // store a new difference-item
                        aItem = new DiffItem();
                        aItem.StartA = startA;
                        aItem.StartB = startB;
                        aItem.DeletedA = lineA - startA;
                        aItem.InsertedB = lineB - startB;
                        a.Add(aItem);
                    }// if
                }// if
            }// while

            result = new DiffItem[a.Count];
            a.CopyTo(result);

            return (result);
        }

        /// <summary>Data on one input file being compared.  
        /// </summary>
        internal class DiffData
        {
            /// <summary>Number of elements (lines).</summary>
            internal int Length { get; set; }

            /// <summary>Buffer of numbers that will be compared.</summary>
            internal int[] Data { get; set; }

            /// <summary>
            /// Array of booleans that flag for modified data.
            /// This is the result of the diff.
            /// This means DeletedA in the first Data or inserted in the second Data.
            /// </summary>
            internal bool[] Modified;

            /// <summary>
            /// Initialize the Diff-Data buffer.
            /// </summary>
            /// <param name="data">reference to the buffer</param>
            internal DiffData(int[] initData)
            {
                this.Data = initData;
                this.Length = initData.Length;
                this.Modified = new bool[Length + 2];
            }// DiffData
        }// class DiffData
    }
}
