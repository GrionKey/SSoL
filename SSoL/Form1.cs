using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSoL
{
    public partial class Form1 : Form
    {
        private int currentGeneration = 0;      // инициализация счеткичка генерации
        public Graphics graphics;
        private int resolution;
        private bool[,] field;
        private int rows, cols;

        public Form1()
        {
            InitializeComponent();
        }
        private void startSimulation()
        {
            if (timer1.Enabled) return;

            currentGeneration = 0;
            Text = $"Generation {currentGeneration}";       // счетчик генерации

            nudResolution.Enabled = false;              // блокирует изменение разрешения когда симуляция запущена
            nudDensity.Enabled = false;                 // блокирует изменение плотности когда симуляция запущена
            resolution = (int)nudResolution.Value;      // присвоить переменной значаение из nudResolution

            rows = pictureBox1.Height / resolution; //
            cols = pictureBox1.Width / resolution;  //

            field = new bool[cols, rows];

            Random random = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);      // инициализация графического обьекта 
            graphics = Graphics.FromImage(pictureBox1.Image);                           // создание обьекта

            timer1.Start();
        }

        private void NextGeneration()
        {
            graphics.Clear(Color.Black);

            var newField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {

                    var neighboursCount = CountNeighbours(x, y);    //кол-во соседей
                    var hasLife = field[x, y];                      //

                    if (!hasLife && neighboursCount == 3) 
                        newField[x, y] = true;
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3)) 
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];

                    if (hasLife)
                        graphics.FillRectangle(Brushes.DarkGreen, x * resolution, y * resolution, resolution - 1, resolution - 1);  // отрисовка клеток
                }
            }
            field = newField;
            pictureBox1.Refresh();
            Text = $"Generation {++currentGeneration}";     // инкременция счетчика с информацией о поколении
        }

        private int CountNeighbours(int x, int y)       // счетчик соседей 
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + cols) % cols;
                    var row = (y + j + rows) % rows;



                    var isSelfCheking = col == x && row == y;
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfCheking)
                        count++;
                }
            }

            return count;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            startSimulation();
        }

        private void StopSimulation()
        {
            if (!timer1.Enabled) return;
            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    var x = e.Location.X / resolution;
                    var y = e.Location.Y / resolution;
                    field[x, y] = true;
                }
            }
            catch { };

            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    var x = e.Location.X / resolution;
                    var y = e.Location.Y / resolution;
                    field[x, y] = false;
                }
            }
            catch { };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = $"Generation {currentGeneration}";       // счетчик генерации
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopSimulation();
        }
    }
}
