using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_6
{
    class Program
    {
        private const int Size = 500;
        private static readonly Random _rnd = new Random();
        static void Main( string[] args )
        {
            var m1 = new int[Size, Size];
            var m2 = new int[Size, Size];
            FillMatrix( ref m1 );
            FillMatrix( ref m2 );

            var m3 = Task.Factory.StartNew( () => MultiplyMatrix( m1, m2 ) );
            var m4 = Task.Factory.StartNew( () => MultiplyMatrixAsync( m1, m2 ) );

            //PrintMatrix(m3.Result);
            //Console.WriteLine("==============================================");
            //PrintMatrix(m4.Result.Result);

            Console.ReadLine();
        }

        private static void FillMatrix( ref int[,] matrix )
        {
            for ( int i = 0; i < Size; i++ )
            {
                for ( int j = 0; j < Size; j++ )
                {
                    matrix[i, j] = _rnd.Next( 0, 10 );
                }
            }
        }

        private static void PrintMatrix( int[,] matrix )
        {
            for ( int i = 0; i < Size; i++ )
            {
                for ( int j = 0; j < Size; j++ )
                {
                    Console.Write( $"{matrix[i, j]}, " );
                }
                Console.Write( "\n" );
            }
        }

        private static int[,] MultiplyMatrix( int[,] m1, int[,] m2 )
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = new int[Size, Size];
            for ( var i = 0; i < Size; i++ )
            {
                for ( var j = 0; j < Size; j++ )
                {
                    for ( var l = 0; l < Size; l++ )
                    {
                        result[i, j] += m1[i, l] * m2[l, j];
                    }
                }
            }
            sw.Stop();
            Console.WriteLine( $"Sync завершено за {sw.Elapsed}" );
            return result;
        }

        private static async Task<int[,]> MultiplyMatrixAsync( int[,] m1, int[,] m2 )
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = new int[Size, Size];
            var tasks = new Task[Size];

            /*Код приводящий к замыканию*/
            //for ( var i = 0; i < Size; i++ )
            //{
            //    tasks[i] = Task.Factory.StartNew( () => CalculateRow( i, m1, m2, ref result ) );
            //}
            /*Код приводящий к замыканию*/
            for ( var i = 0; i < Size; i++ )
            {
                var t = i;
                tasks[i] = Task.Factory.StartNew( () => CalculateRow( t, m1, m2, ref result ) );
            }
            await Task.WhenAll( tasks );

            sw.Stop();
            Console.WriteLine( $"Async завершено за {sw.Elapsed}" );

            return result;
        }

        private static void CalculateRow( int i, int[,] m1, int[,] m2, ref int[,] result )
        {
            for ( var j = 0; j < Size; j++ )
            {
                for ( var l = 0; l < Size; l++ )
                {
                    result[i, j] += m1[i, l] * m2[l, j];
                }
            }
        }
    }
}
