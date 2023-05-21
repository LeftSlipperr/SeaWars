using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая_Морской_бой
{
    public abstract class Maps
    {
        public int[,] myMap = new int[GameLogic.mapSize, GameLogic.mapSize];//карта бота
        public int[,] enemyMap = new int[GameLogic.mapSize, GameLogic.mapSize];//карта игрока
    }

    public class Bot : Maps
    {

        public Button[,] myButtons = new Button[GameLogic.mapSize, GameLogic.mapSize];
        public Button[,] enemyButtons = new Button[GameLogic.mapSize, GameLogic.mapSize];

        public Bot(int[,] myMap, int[,] enemyMap, Button[,] myButtons, Button[,] enemyButtons)
        {
            this.myMap = myMap;
            this.enemyMap = enemyMap;
            this.enemyButtons = enemyButtons;
            this.myButtons = myButtons;
        }
        public bool IsGameFinished()
        {

            for (int i = 0; i < GameLogic.mapSize; i++)
            {
                for (int j = 0; j < GameLogic.mapSize; j++)
                {
                    if (myMap[i, j] == 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public bool IsInsideMap(int i, int j)// если какой либо из параметров выходит за границу карты возвращаем false, функция для предотвращения выхода за границу
        {
            if (i < 0 || j < 0 || i >= GameLogic.mapSize || j >= GameLogic.mapSize)
            {
                return false;
            }
            return true;
        }

        public bool IsEmpty(int i, int j, int length)
        {
            bool isEmpty = true;
            try
            {
                for (int k = j; k < j; k++)
                {
                    if (myMap[i, k] != 0)
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            return isEmpty;
        }

        private bool NoAdjacentShips(int posX, int posY, int lengthShip)
        {
            int mapSize = GameLogic.mapSize;

            // Проверка по вертикали
            for (int i = posY - 1; i <= posY + lengthShip; i++)
            {
                for (int j = posX - 1; j <= posX + 1; j++)
                {
                    if (i >= 0 && i < mapSize && j >= 0 && j < mapSize)
                    {
                        if (myMap[j, i] == 1)
                        {
                            return false;
                        }
                    }
                }
            }

            // Проверка по горизонтали
            for (int i = posX - 1; i <= posX + lengthShip; i++)
            {
                for (int j = posY - 1; j <= posY + 1; j++)
                {
                    if (i >= 0 && i < mapSize && j >= 0 && j < mapSize)
                    {
                        if (myMap[i, j] == 1)
                        {
                            return false;
                        }
                    }
                }
            }

            // Проверка по диагонали
            for (int i = posX - 1; i <= posX + lengthShip; i++)
            {
                for (int j = posY - 1; j <= posY + lengthShip; j++)
                {
                    if (i >= 0 && i < mapSize && j >= 0 && j < mapSize)
                    {
                        if (myMap[i, j] == 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        public int[,] ConfigureShips() // расставление кораблей для бота
        {

            int lengthShip = 4;
            int cycleValue = 4;
            int shipsCount = 10;
            Random r = new Random();



            while (shipsCount > 0)
            {
                for (int i = 0; i < cycleValue / 4; i++)
                {
                    int posX = 0;
                    int posY = 0;

                    bool validPosition = false;

                    while (!validPosition)
                    {
                        posX = r.Next(1, GameLogic.mapSize);
                        posY = r.Next(1, GameLogic.mapSize);

                        // Проверка, чтобы корабли не располагались рядом с другими кораблями
                        if (IsEmpty(posX, posY, lengthShip) && NoAdjacentShips(posX, posY, lengthShip))
                        {
                            validPosition = true;
                        }
                    }

                    while (!IsInsideMap(posX, posY + lengthShip - 1) || !IsEmpty(posX, posY, lengthShip))
                    {
                        posX = r.Next(1, GameLogic.mapSize);
                        posY = r.Next(1, GameLogic.mapSize);
                    }
                    for (int k = posY; k < posY + lengthShip; k++)
                    {
                        myMap[posX, k] = 1;
                    }

                    shipsCount--;
                    if (shipsCount <= 0)
                    {
                        break;
                    }

                }
                cycleValue += 4;
                lengthShip--;
            }
            return myMap;
        }


        public bool Shoot()
        {
            bool hit = false;
            Random r = new Random();

            int posX = r.Next(1, GameLogic.mapSize);
            int posY = r.Next(1, GameLogic.mapSize);

            while (enemyButtons[posX, posY].BackColor == Color.Blue || enemyButtons[posX, posY].BackColor == Color.Black)
            {
                posX = r.Next(1, GameLogic.mapSize);
                posY = r.Next(1, GameLogic.mapSize);
            }

            if (enemyMap[posX, posY] != 0)
            {
                hit = true;
                enemyMap[posX, posY] = 0;
                enemyButtons[posX, posY].BackColor = Color.Blue;
                enemyButtons[posX, posY].Text = "X";
            }
            else
            {
                hit = false;
                enemyButtons[posX, posY].BackColor = Color.Black;
            }
            if (hit)
                Shoot();
            return hit;
        }
    }
}
