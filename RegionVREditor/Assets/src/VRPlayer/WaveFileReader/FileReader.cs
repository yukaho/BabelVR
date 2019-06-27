using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


public class FileReader
{
    protected string file_name;
    protected string file_dir;
    protected byte[] data_array;

    protected virtual void Read(string file_dir)
    {
        //reference to file directory
        this.file_dir = @file_dir;

        string[] spit = file_dir.Split('/');
        this.file_name = spit[spit.Length - 1];




        try
        {

            using (FileStream fsSource = new FileStream(file_dir,
                FileMode.Open, FileAccess.Read))
            {

                // Read the source file into a byte array.
                Console.WriteLine("Reading file from " + file_dir);
                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = bytes.Length;

                Console.WriteLine("Read File Complete. " + numBytesToRead + " Bytes.");

                //reference to the bytes
                data_array = bytes;

            }
        }
        catch (FileNotFoundException ioEx)
        {
            Console.WriteLine(ioEx.Message);
        }
    }

    protected byte[] getByteArray()
    {
        return data_array;
    }

    protected byte[] getByteArray(int offset, int length)
    {
        byte[] return_array = new byte[length];

        int count = 0;
        for (int i = offset; i < length; i++)
        {
            return_array[count] = data_array[i];
            count++;
        }


        return return_array;
    }

    public string getFileName()
    {
        return file_name;
    }
}

