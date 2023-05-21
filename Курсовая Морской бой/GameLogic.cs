using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая_Морской_бой
{
    public class GameLogic : Form, IStart
    {
        public const int mapSize = 10; // размерность карты
        public int cellSize = 30; // размерность ячейки
        public string alphabet = "АБВГДЕЖЗИК"; // буквы по краям
        public bool isHorizontal = true; // по умолчанию горизонтальная ориентация

        public int[] a = new int[10];
        public int[,] myMap = new int[mapSize, mapSize]; // размерность карты
        public int[,] enemyMap = new int[mapSize, mapSize]; // размерность вражеской карты
        ComboBox comboBoxShipSize = new ComboBox();
        ComboBox comboBoxHorizontalVertical = new ComboBox();

        public Button[,] myButtons = new Button[mapSize, mapSize]; //хранение кнопок игрока
        public Button[,] enemyButtons = new Button[mapSize, mapSize]; //хранение кнопок врага

        public bool isPlaying = false; //изначально не игровой режим

        public Bot bot;

        private int placedFourDeckShips = 0;
        private int placedThreeDeckShips = 0;
        private int placedTwoDeckShips = 0;
        private int placedOneDeckShips = 0;

        private int maxFourDeckShips = 1;
        private int maxThreeDeckShips = 2;
        private int maxTwoDeckShips = 4;
        private int maxOneDeckShips = 6;


        string IStart.Messagestart()
        {
            string a = "Игра началась";
            return a;
        }
        string IStart.MessageRefresh()
        {
            string a = "Игра началась заново";
            return a;
        }

        public void Init()
        {
            isPlaying = false;
            CreateMaps();
            bot = new Bot(enemyMap, myMap, enemyButtons, myButtons);
            enemyMap = bot.ConfigureShips();


            placedFourDeckShips = 0;
            placedThreeDeckShips = 0;
            placedTwoDeckShips = 0;
            placedOneDeckShips = 0;

            maxFourDeckShips = 1;
            maxThreeDeckShips = 2;
            maxTwoDeckShips = 4;
            maxOneDeckShips = 6;
        }

        public void CreateMaps()
        {
            try
            {
                this.Width = mapSize * 2 * cellSize + 50; // размерность формы
                this.Height = (mapSize + 3) * cellSize + 100;
                for (int i = 0; i < mapSize; i++) // циклы для создания поля
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        myMap[i, j] = 0;

                        Button button = new Button(); // создание ячеек для поля и размерность
                        button.Location = new Point(j * cellSize, i * cellSize);
                        button.Size = new Size(cellSize, cellSize);
                        button.BackColor = Color.White; // стандартный цвет белый
                        button.FlatStyle = FlatStyle.Popup;
                        if (j == 0 || i == 0)
                        {
                            button.BackColor = Color.Gray; // кнопки по краям не активные серые для букв и цифр
                            if (i == 0 && j > 0)
                                button.Text = alphabet[j - 1].ToString(); // заполнение кнопок по краям
                            if (j == 0 && i > 0)
                                button.Text = i.ToString();
                        }
                        else
                        {
                            button.Click += new EventHandler(ConfigureShips); // событие конфигурация кораблей
                        }
                        myButtons[i, j] = button;
                        this.Controls.Add(button);
                    }
                }
                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        myMap[i, j] = 0;
                        enemyMap[i, j] = 0;

                        Button button = new Button();
                        button.Location = new Point(320 + j * cellSize, i * cellSize);
                        button.Size = new Size(cellSize, cellSize);
                        button.BackColor = Color.White;
                        button.FlatStyle = FlatStyle.Popup;
                        if (j == 0 || i == 0)
                        {
                            button.BackColor = Color.Gray;
                            if (i == 0 && j > 0)
                                button.Text = alphabet[j - 1].ToString();
                            if (j == 0 && i > 0)
                                button.Text = i.ToString();
                        }
                        else
                        {
                            button.Click += new EventHandler(PlayerShoot);
                        }
                        enemyButtons[i, j] = button; // кнопки игрока
                        this.Controls.Add(button); // добавление ячеек
                    }
                }

                Label map1 = new Label();
                map1.Text = "Карта игрока";
                map1.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 10);
                this.Controls.Add(map1);

                Label map2 = new Label();
                map2.Text = "Карта противника";
                map2.Location = new Point(350 + mapSize * cellSize / 2, mapSize * cellSize + 10);
                this.Controls.Add(map2);

                Button startButton = new Button();
                startButton.Text = "Начать";
                startButton.Click += new EventHandler(Start);
                startButton.Location = new Point(0, mapSize * cellSize + 20);
                startButton.FlatStyle = FlatStyle.Flat;
                this.Controls.Add(startButton);

                Button refreshButton = new Button();
                refreshButton.Text = "Заново";
                refreshButton.Click += new EventHandler(Refresh);
                refreshButton.Location = new Point(0, mapSize * cellSize + 50);
                refreshButton.FlatStyle = FlatStyle.Flat;
                this.Controls.Add(refreshButton);

                Label shipSize = new Label();
                shipSize.Text = "Выбор размера корабля";
                shipSize.Location = new Point(130, 335);
                this.Controls.Add(shipSize);

                comboBoxShipSize.Items.Clear();
                comboBoxShipSize.Items.Add(1);
                comboBoxShipSize.Items.Add(2);
                comboBoxShipSize.Items.Add(3);
                comboBoxShipSize.Items.Add(4);
                comboBoxShipSize.Location = new Point(115, 360);
                this.Controls.Add(comboBoxShipSize);

                Label horizontalVertical = new Label();
                horizontalVertical.Text = "Направление";
                horizontalVertical.Location = new Point(130, 387);
                this.Controls.Add(horizontalVertical);

                comboBoxHorizontalVertical.Items.Clear();
                comboBoxHorizontalVertical.Items.Add("Горизонтально");
                comboBoxHorizontalVertical.Items.Add("Вертикально");
                comboBoxHorizontalVertical.Location = new Point(115, 410);
                this.Controls.Add(comboBoxHorizontalVertical);

                comboBoxHorizontalVertical.SelectedIndexChanged += (sender, e) =>
                {
                    if (comboBoxHorizontalVertical.SelectedIndex == 0)// 0 - горизонтальная ориентация, 1 - вертикальная ориентация
                        isHorizontal = true;
                    else if (comboBoxHorizontalVertical.SelectedIndex == 1)
                        isHorizontal = false;
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Start(object sender, EventArgs e) // обработчик для включение игрового режима
        {
            isPlaying = true;
            MessageBox.Show(((IStart)this).Messagestart());
        }

        public void Refresh(object sender, EventArgs e)
        {
            MessageBox.Show(((IStart)this).MessageRefresh());
            this.Controls.Clear();
            Init();
        }

        public class Ship
        {
            public int Length { get; private set; }
            public List<Point> Coordinates { get; private set; }

            public Ship(int length)
            {
                Length = length;
                Coordinates = new List<Point>();
            }

        }

        public void ConfigureShips(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            if (!isPlaying)
            {
                int row = pressedButton.Location.Y / cellSize;
                int col = pressedButton.Location.X / cellSize;


                if (myMap[row, col] == 0) // Если ячейка пустая
                {
                    ShipSize selectedShipSize = (ShipSize)comboBoxShipSize.SelectedIndex + 1; // Размерность корабля из ComboBox

                    if (CanPlaceShip(row, col, selectedShipSize)) // Проверка, можно ли разместить корабль
                    {
                        PlaceShip(row, col, selectedShipSize); // Размещение корабля

                        // Изменение цвета ячеек корабля
                        for (int i = 0; i < (int)selectedShipSize; i++)
                        {
                            Button cellButton = GetButtonByLocation(row + (isHorizontal ? 0 : i), col + (isHorizontal ? i : 0)); // учет ориентации
                            cellButton.BackColor = Color.Red;
                        }

                        // Уменьшение доступного количества кораблей выбранного размера
                        switch (selectedShipSize)
                        {
                            case ShipSize.FourDeck:
                                placedFourDeckShips++;
                                if (placedFourDeckShips >= maxFourDeckShips)
                                {
                                    maxOneDeckShips--;
                                    comboBoxShipSize.Items.Remove(ShipSize.OneDeck);
                                    if (comboBoxShipSize.SelectedIndex == 3)
                                        comboBoxShipSize.SelectedIndex = 2;
                                }
                                break;
                            case ShipSize.ThreeDeck:
                                placedThreeDeckShips++;
                                if (placedThreeDeckShips >= maxThreeDeckShips)
                                {
                                    maxTwoDeckShips--;
                                    comboBoxShipSize.Items.Remove(ShipSize.TwoDeck);
                                    if (comboBoxShipSize.SelectedIndex == 2)
                                        comboBoxShipSize.SelectedIndex = 1;
                                }
                                break;
                            case ShipSize.TwoDeck:
                                placedTwoDeckShips++;
                                if (placedTwoDeckShips >= maxTwoDeckShips)
                                {
                                    maxOneDeckShips--;
                                    comboBoxShipSize.Items.Remove(ShipSize.OneDeck);
                                    if (comboBoxShipSize.SelectedIndex == 1)
                                        comboBoxShipSize.SelectedIndex = 0;
                                }
                                break;
                            case ShipSize.OneDeck:
                                placedOneDeckShips++;
                                if (placedOneDeckShips >= maxOneDeckShips)
                                {
                                    comboBoxShipSize.Items.Remove(ShipSize.OneDeck);
                                    if (comboBoxShipSize.SelectedIndex == 0)
                                        comboBoxShipSize.SelectedIndex = -1;
                                }
                                break;
                        }

                        // Проверка, все ли корабли размещены
                        if (placedFourDeckShips == maxFourDeckShips &&
                            placedThreeDeckShips == maxThreeDeckShips &&
                            placedTwoDeckShips == maxTwoDeckShips &&
                            placedOneDeckShips == maxOneDeckShips)
                        {
                            MessageBox.Show("Все корабли размещены!");
                            comboBoxShipSize.Items.Clear();
                        }
                    }
                }
                else // Если в ячейке уже есть корабль
                {
                    pressedButton.BackColor = Color.White;
                    myMap[row, col] = 0;
                }
            }
        }




        // Проверяет, можно ли разместить корабль в заданных координатах
        private bool CanPlaceShip(int startRow, int startCol, ShipSize size)
        {
            if (comboBoxHorizontalVertical.SelectedIndex == 0)
            {
                // Проверка горизонтального расположения корабля
                if (startCol + (int)size > mapSize)
                    return false;

                for (int c = startCol; c < startCol + (int)size; c++)
                {
                    if (myMap[startRow, c] != 0)
                        return false;

                    if (startRow > 0 && myMap[startRow - 1, c] != 0)
                        return false;

                    if (startRow < mapSize - 1 && myMap[startRow + 1, c] != 0)
                        return false;

                    if (c > 0 && myMap[startRow, c - 1] != 0)
                        return false;

                    if (c < mapSize - 1 && myMap[startRow, c + 1] != 0)
                        return false;
                }

                return true;
            }
            else if (comboBoxHorizontalVertical.SelectedIndex == 1)
            {
                if (startRow + (int)size > mapSize)
                    return false;

                for (int r = startRow; r < startRow + (int)size; r++)
                {
                    if (myMap[r, startCol] != 0)
                        return false;

                    if (startCol > 0 && myMap[r, startCol - 1] != 0)
                        return false;

                    if (startCol < mapSize - 1 && myMap[r, startCol + 1] != 0)
                        return false;

                    if (r > 0 && myMap[r - 1, startCol] != 0)
                        return false;

                    if (r < mapSize - 1 && myMap[r + 1, startCol] != 0)
                        return false;
                }

                return true;
            }
            else
            {
                MessageBox.Show("Выберете как вы хотите расставлять корабли горизонтально или вертикально!");
                return false;
            }
           

        }

        // Размещает корабль в заданных координатах
        private void PlaceShip(int startRow, int startCol, ShipSize size)
        {
            // Горизонтальное расположение корабля
            if (startCol + (int)size <= mapSize)
            {
                for (int c = startCol; c < startCol + (int)size; c++)
                {
                    myMap[startRow, c] = 1;
                }
            }
        }

        // Получает кнопку по координатам
        private Button GetButtonByLocation(int row, int col)
        {
            foreach (Control control in Controls)
            {
                if (control is Button button && button.Location.Y / cellSize == row && button.Location.X / cellSize == col)
                {
                    return button;
                }
            }
            return null;
        }

        enum ShipSize
        {
            OneDeck = 1,
            TwoDeck = 2,
            ThreeDeck = 3,
            FourDeck = 4
        }
        public bool CheckIfMapIsNotEmpty() // проверка на то что пустая ли карта
        {
            bool isEmpty1 = true;
            bool isEmpty2 = true;
            for (int i = 1; i < mapSize; i++)
            {
                for (int j = 1; j < mapSize; j++)
                {
                    if (myMap[i, j] != 0)
                        isEmpty1 = false;
                    if (enemyMap[i, j] != 0)
                        isEmpty2 = false;
                }
            }
            if (isEmpty1)
            {
                MessageBox.Show("Bot Win");
                return false;

            }
            else if (isEmpty2)
            {
                MessageBox.Show("Player Win");
                return false;

            }
            else return true;

        }

        public void PlayerShoot(object sender, EventArgs e) // выстрел игрока
        {
            Button pressedButton = sender as Button;
            bool playerTurn = Shoot(enemyMap, pressedButton); // ход игрока передаем нажатие кнопки и карту игрока
           
            if (!playerTurn && isPlaying) // если ход противника, то вызывается функция shoot для бота
                bot.Shoot();

            if (!CheckIfMapIsNotEmpty()) // если карта не пустая очистка
            {
                this.Controls.Clear();
                Init();
            }
            
        }

        public bool Shoot(int[,] map, Button pressedButton) // функция выстрела
        {
            bool hit = false; // false = не попал
            if (isPlaying)
            {
                int delta = 0;
                if (pressedButton.Location.X > 320)
                    delta = 320;
                if (map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] != 0) // условие в случае попадания по кораблю
                {
                    hit = true;
                    map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 0;
                    pressedButton.BackColor = Color.Blue;
                    pressedButton.Text = "X";
                }
                else // иначе не попал
                {
                    hit = false;

                    pressedButton.BackColor = Color.Black;
                }
            }
            return hit;
        }
    }
}