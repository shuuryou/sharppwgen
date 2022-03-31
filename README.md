First [Theodore Ts'o](https://github.com/tytso/pwgen/blob/master/pwgen.c) once wrote it in C.

Then [Kazuyoshi Kato](https://github.com/kzys/pwgen-js/) ported it to JavaScript some time ago.

One day I needed it in C# and well, here we are.

How to use:

```cs
using pwgen;
using System;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            // example: Erie5zet
            Console.WriteLine(SharpPwgen.Generate());

            // example: uhai1Jahngizaeto
            Console.WriteLine(SharpPwgen.Generate(16));

            // example: caigophi
            Console.WriteLine(SharpPwgen.Generate(8, false, false));
        }
    }
}
```

Enjoy.