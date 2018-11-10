using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lessson_6
{
    class Program
    {
        private static readonly Random Rnd = new Random();
        private const int FileNum = 100;
        private const string Path = @"D:\Projects\C# Level 3\Lesson 6\Support\";
        private static readonly string ResultFile = $@"{Path}\result.dat";
        static void Main( string[] args )
        {
            RunFileCal();

            Console.WriteLine( "Done" );
            Console.ReadLine();
        }

        private static async void RunFileCal()
        {
            ClearResult( ResultFile, $@"{Path}\Files" );

            await CreateFilesAsync( FileNum, Path );
            await CalculateFilesAsync( Path, ResultFile );
        }

        private static void ClearResult( string resultFileDir, string fileDir )
        {
            var file = new FileInfo( resultFileDir );
            if ( file.Exists ) file.Delete();

            var dir = new DirectoryInfo( fileDir );
            if ( dir.Exists ) dir.Delete( true );
        }

        private static async Task CreateFilesAsync( int fileNum, string path )
        {
            var dir = new DirectoryInfo( path );
            var tasks = new Task[fileNum];

            for ( int i = 0; i < fileNum; i++ )
            {
                var t = i;
                tasks[i] = Task.Factory.StartNew( () => CreateFileAsync( t, path ) );
            }
            await Task.WhenAll( tasks );

            /*Код приводящий к замыканию*/
            //for ( int i = 0; i < fileNum; i++ )
            //{
            //    tasks[i] = Task.Factory.StartNew( () => CreateFileAsync( i, path ) );
            //}
            /*Код приводящий к замыканию*/
        }

        private static async void CreateFileAsync( int i, string path )
        {
            var dirInfo = new DirectoryInfo( $@"{path}\Files\" );
            if ( !dirInfo.Exists ) dirInfo.Create();

            var file = new FileInfo( $@"{path}\Files\file{i}.txt" ).CreateText();
            await file.WriteLineAsync(
                $"{Rnd.Next( 1, 2 )} {Rnd.NextDouble() * Rnd.Next( 1, 10 )} {Rnd.NextDouble() * Rnd.Next( 1, 10 )}" );
            await file.FlushAsync();
            file.Close();
        }

        private static async Task CalculateFilesAsync( string path, string resultFile )
        {
            var fileInfo = new DirectoryInfo( $@"{path}\Files" ).GetFiles();
            foreach ( var file in fileInfo )
            {
                var res = await Task.Factory.StartNew( () => CalculateFile( file ) );
                WriteResult( resultFile, res );
            }
        }

        private static string CalculateFile( FileInfo file )
        {
            var stream = file.OpenText();
            var data = stream.ReadLine()?.Split( ' ' );

            double result = 0;
            var strSign = "";
            var sign = int.Parse( data[0] );
            var firstNum = double.Parse( data[1] );
            var secondNum = double.Parse( data[2] );

            switch ( sign )
            {
                case 1:
                    strSign = "*";
                    result = firstNum * secondNum;
                    break;
                case 2:
                    strSign = @"/";
                    result = firstNum / secondNum;
                    break;
            }

            return $"{DateTime.Now}\tFile {file.Name}\t{firstNum} {strSign} {secondNum} = {result}";
        }

        private static async void WriteResult( string path, string result )
        {
            var file = File.AppendText( path );
            await file.WriteLineAsync( result );
            file.Close();
        }
    }
}
