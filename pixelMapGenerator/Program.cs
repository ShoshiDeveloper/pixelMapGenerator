using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

/*
 * 
 Программа-генератор карты, которая генерирует 2х мерный массив карты и переводит его в картинку, где
0 - вода (синий)
1 - песок (жёлтый)
2 - земля/трава (зелёный)
 */

namespace pixelMapGenerator
{
    class Program
    {
        const int waterProbability = 100, dirtProbability = 100;//вероятности
        const int height = 500, width = 500;
        static int[,] map = new int[height, width];//строки * столбцы (ячейки в строке)

        static void Main(string[] args)
        {
            GenerateMap();
            GenerateAndSaveMapPicture();
        }

        static int GetMapElementOfProbability(int probabilityPriorityType, int priorityType, int secondType) {
            if (new Random().Next(1, 101) < probabilityPriorityType)
            {
                return priorityType;
            }
            else {
                return secondType;
            }
        }

        static void GenerateMap()
        {
            Console.WriteLine($"Начал генерацию");
            Random random = new Random();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = random.Next(0, 3);//сработает в тех случаях, когда нет разницы, что появится
                    if (i == 0)//если это первая строка
                    {
                        if (j != 0)//элемент не первый
                        {
                            if (map[i, j - 1] == 0)//если предыдущий элемент вода, то
                            {
                                map[i, j] = GetMapElementOfProbability(waterProbability,0,1);//спавним либо воду, либо песок

                            } else if (map[i,j-1] == 2)
                            {//если пред. элем. земля, то
                                map[i, j] = GetMapElementOfProbability(dirtProbability, 2, 1);//либо песок, либо земля
                            }
                        }
                    }
                    else { //если строка не первая
                        if (j == 0)//первый элемент в строке
                        {
                            if (map[i-1, j] == 0)//верхний элемент - вода, то
                            {
                                map[i, j] = GetMapElementOfProbability(waterProbability,0,1);//данный элемент либо вода, либо песок

                            } else if (map[i-1, j] == 2)//верхний эл. - земля
                            {
                                map[i, j] = GetMapElementOfProbability(dirtProbability,2,1);//либо песок, либо земля
                            }
                        }
                        else { //если элемент не первый в строке
                            /*
                             тут есть несколько вариаций (1е слово - эл-т слева, 2е слово - сверху):
                            вода, вода - вода/ песок +

                            песок, песок - неважно, что будет, а значит можно не делать проверку

                            вода, песок - вода/ песок +
                            песок, вода - песок/ вода +

                            вода, земля - может быть только песок +
                            земля, вода - такого не должно быть

                            песок, земля - песок / земля +
                            земля, песок - земля/ песок +
                            земля, земля - песок/ земля +

                             */
                            if ((map[i, j - 1] == 0 && map[i - 1, j] == 0) || (map[i, j - 1] == 0 && map[i - 1, j] == 1) || (map[i, j - 1] == 1 && map[i - 1, j] == 0)) {
                                map[i, j] = GetMapElementOfProbability(waterProbability,0,1);//вода/ песок
                            } else if ((map[i, j - 1] == 2 && map[i - 1, j] == 2) || (map[i, j - 1] == 1 && map[i - 1, j] == 2) || (map[i, j - 1] == 2 && map[i - 1, j] == 1))
                            {
                                map[i, j] = GetMapElementOfProbability(dirtProbability,2,1);//песок/ земля
                            } else if (map[i, j - 1] == 0 && map[i - 1, j] == 2) {
                                map[i, j] = 1;//только песок
                            }
                        }
                    }
                    
                }
/*                Console.WriteLine($"Строка {i} сгенерирована");*/
            }
        }

        static void GenerateAndSaveMapPicture() {
            Console.WriteLine("Магия! Превращаю генерацию в картинку");
            //создадим новую кисть, и прямоугольник, чтобы не генерировать каждый раз эти объекты
            Pen pen = new Pen(Color.Black);
            Rectangle rectangle = new Rectangle();

            Bitmap pic = new Bitmap(height, width);//new picture 400x400 px
            Graphics image = Graphics.FromImage(pic);//Graphics of pic
            image.FillRectangle(new SolidBrush(Color.White), 0, 0, height, width);

            for (int i = 0; i < height; i ++)
            {
                for (int j = 0; j < width; j ++)
                {
                    switch (map[i, j])
                    {
                        case 0:
                            pen.Color = Color.Blue;
                            break;
                        case 1:
                            pen.Color = Color.Yellow;

                            break;
                        case 2:
                            pen.Color = Color.Green;

                            break;
                        default:
                            pen.Color = Color.Black;
                            break;
                    }
                    rectangle.Location = new Point(i, j);//новые координаты для прямоугольника
                    rectangle.Size = new Size(1, 1);//1х1 - это пиксель
                    image.DrawRectangle(pen, rectangle);
                }
            }
            pic.Save("C:\\VSCodeProjects\\pixelMapGenerator\\ge.jpg");
            Console.WriteLine("Я закончил, держи картинку");
            Process.Start(new ProcessStartInfo("C:\\VSCodeProjects\\pixelMapGenerator\\ge.jpg") { UseShellExecute = true });
        }

    }
}
