using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;


namespace _2048
{
    public partial class Game : Form
    {
        #region 变量
        int size = 85;//方块大小
        int left = 110;//方阵左边边距
        int top = 110;//方阵上边边距
        const int n = 4;//方阵的大小
        int image_num = 2;//生成数字的基数
        int box_number = 0;//现存方块个数
        int score = 0;//得分
        bool ok = false;//用来记录该次操作是否有方块移动或者消除，如果都无则不予以生成新方块
        Random ran = new Random();//随机数对象
        SoundPlayer music = new SoundPlayer("music\\click.wav");//音乐播放器对象
        PictureBox[,] pictureBox = new PictureBox[n, n];//申请二维PictureBox组，用于构建游戏主界面
        #endregion
        public Game()
        {
            InitializeComponent();

        }     
        //随机产生2或4的方块
        public void random_creat()
        {        
            int ii = ran.Next(4) ;//在4*4的方格内随机生成一对坐标
            int jj = ran.Next(4) ;
            if (pictureBox[ii, jj].Visible == false)//如果该位置没有方块，则生成一个新的方块
            {
                pictureBox[ii, jj].Visible = true;
                if (ran.Next(2)== 0)
                {
                    pictureBox[ii, jj].Tag = image_num;//图片的命名即为其所表示的数字大小
                    pictureBox[ii, jj].Image = Image.FromFile(Application.StartupPath + @"\image\" + pictureBox[ii, jj].Tag + ".jpg");
                }
                else
                {
                    pictureBox[ii, jj].Tag = (image_num * 2);
                    pictureBox[ii, jj].Image = Image.FromFile(Application.StartupPath + @"\image\" + pictureBox[ii, jj].Tag + ".jpg");
                }
                ++box_number;//方块个数加一        
            }
            else
            {    
                 random_creat();//如果该位置有方块，则重新生成一次坐标。
            }          
        }
        //合并方块，将[i1,j1]处的方块合并到[i,j]处
        public void merge(int i,int j,int i1,int j1)
        {
            ok = true;//记录有消除操作
            int temp =int.Parse (pictureBox[i, j].Tag.ToString ());
            pictureBox[i, j].Tag =(2*temp);//更新标签
            pictureBox[i, j].Image = Image.FromFile(Application.StartupPath + @"\image\" + pictureBox[i, j].Tag + ".jpg");//更新图片
            --box_number;//消除一个方块
            pictureBox[i1, j1].Visible = false;//消除的位置可视性置为false
            score += temp * 2;//更新分数
            label3.Text = score.ToString ();//更新显示框
            music.Play();//播放消除音效
            if (int.Parse(label3.Text) > int.Parse(label4.Text))//当前得分超过最高分了，更新最高分。
            {
                label4.Text = label3.Text;
            }
            if (temp >= 1024)//合成出了2048
            {
                MessageBox.Show("恭喜通关！！！");//弹出消息盒子
            }
        }
        //移动方块，将[i1,j1]处的方块合并到[i,j]处
        public bool move(int i, int j, int i1, int j1)
        {
            if (pictureBox[i, j].Visible == true) return false;//原位置有方块，返回false，移动不成功
            pictureBox[i, j].Image = Image.FromFile(Application.StartupPath + @"\image\" + pictureBox[i1, j1].Tag + ".jpg");//图片移动
            pictureBox[i, j].Visible = true ;//可视性移动
            pictureBox[i1, j1].Visible = false;
            pictureBox[i, j].Tag = pictureBox[i1, j1].Tag;//标签移动
            ok = true;//记录有移动操作
            return true ;//移动成功
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int d = e.KeyValue;       
            switch (d)
            {
                #region 向左
                case 37:                           
                        for (int i = 0; i < n; ++i)//循环每一行
                        {                           
                            int temp = 0;//假设空位置为0
                            for (int j = 1; j < n; ++j)
                            {
                                if (pictureBox[i, j].Visible == true)
                                {
                                    //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                    while (temp < j && move(i, temp++, i, j) == false) ;                                   
                                }
                            }
                            //相邻且相等的合并相消
                            for (int j = 0; j < n; ++j)
                            {

                                if (j < n - 1 && pictureBox[i, j].Visible == true && pictureBox[i, j + 1].Visible == true)//相邻的两个位置都有方块
                                {
                                    if (int.Parse(pictureBox[i, j].Tag.ToString()) == int.Parse(pictureBox[i, j + 1].Tag.ToString())
                                        && pictureBox[i, j].Tag != null && pictureBox[i, j + 1].Tag != null)//满足标签一致且不为NULL
                                    {
                                        merge(i, j, i, j + 1);//合并
                                    }
                                }
                            }
                            //合并之后再次移动，因为合并完成后可能会出现其他空位
                            temp = 0;
                            for (int j = 1; j < n; ++j)
                            {
                                if (pictureBox[i, j].Visible == true)
                                {
                                    //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                    while (temp < j && move(i, temp++, i, j) == false) ;
                                }
                            }
                        }                 
                    break;
                #endregion
                #region 上
                case 38:
                    for (int j = 0; j < n; ++j)
                    {
                        int temp = 0;//假设空位置为0
                        for (int i = 1; i < n; ++i)
                        {
                            if (pictureBox[i, j].Visible == true)
                            {
                                //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                while (temp < i && move(temp++, j, i, j) == false) ;
                            }
                        }
                        for (int i = 0; i < n; ++i)
                        {
                            if (i < n - 1 && pictureBox[i, j].Visible == true && pictureBox[i + 1, j].Visible == true)
                            {
                                if (int.Parse(pictureBox[i, j].Tag.ToString()) == int.Parse(pictureBox[i + 1, j].Tag.ToString())
                                    && pictureBox[i, j].Tag != null && pictureBox[i + 1, j].Tag != null)
                                {
                                    merge(i, j, i + 1, j);
                                }
                            }
                        }
                        temp = 0;
                        for (int i = 1; i < n; ++i)
                        {
                            if (pictureBox[i, j].Visible == true)
                            {
                                //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                while (temp < i && move(temp++, j, i, j) == false) ;
                            }
                        }
                    }
                    break;
                #endregion
                #region 右
                case 39:    
                        for (int i = 0; i < n; ++i)
                        {
                            int temp = n - 1;//假设空位置为0
                            for (int j = n-2; j >= 0; --j)
                            {
                                if (pictureBox[i, j].Visible == true)
                                {
                                    //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                    while (temp >j && move(i, temp--, i, j) == false) ;
                                }
                            }
                            //相邻且相等的合并相消
                            for (int j = n - 1; j >= 0; j--)
                            {

                                if (j > 0 && pictureBox[i, j].Visible == true && pictureBox[i, j - 1].Visible == true)
                                {
                                    if (int.Parse(pictureBox[i, j].Tag.ToString()) == int.Parse(pictureBox[i, j - 1].Tag.ToString())
                                        && pictureBox[i, j].Tag != null && pictureBox[i, j - 1].Tag != null)
                                    {
                                        merge(i, j, i, j - 1);
                                    }
                                }
                            }
                            temp = n - 1;
                            for (int j = n - 2; j >= 0; --j)
                            {
                                if (pictureBox[i, j].Visible == true)
                                {
                                    //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                    while (temp > j && move(i, temp--, i, j) == false) ;
                                }
                            }
                        }        
                    break;
                #endregion
                #region 下
                case 40:
                        for (int j = 0; j < n; ++j)
                        {
                            int temp = n-1;
                            for (int i = n - 2; i >= 0; i--)
                            {
                                if (pictureBox[i, j].Visible == true)
                                {
                                    //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                    while (temp > i && move(temp--,j , i, j) == false) ;
                                }
                            }

                            for (int i = n - 1; i >= 0; i--)
                            {
                                if (i > 0 && pictureBox[i, j].Visible == true && pictureBox[i - 1, j].Visible == true)
                                {
                                    if (int.Parse(pictureBox[i, j].Tag.ToString()) == int.Parse(pictureBox[i - 1, j].Tag.ToString())
                                        && pictureBox[i, j].Tag != null && pictureBox[i - 1, j].Tag != null)
                                    {
                                        merge(i, j, i - 1, j);
                                    }
                                }
                            }
                            temp = n - 1;
                            for (int i = n - 2; i >= 0; i--)
                            {
                                if (pictureBox[i, j].Visible == true)
                                {
                                    //当未移动到最顶端，且该空位置有其他方块时，换下一个空位置。
                                    while (temp > i && move(temp--, j, i, j) == false) ;
                                }
                            }
                        }
                    
                    
                    break;
                #endregion
                default: break;
            }
            if (box_number >= Math.Pow(n, 2))//如果格子都被沾满则游戏结束
            {
                File.WriteAllText("score.txt", label4.Text);//最高分写入
                MessageBox.Show("Game Over!!!");

            }
            else if(ok==true )//未被占满则游戏继续，生成新方块
            {
                random_creat();
                ok = false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("score.txt"))
            {
                label4.Text = File.ReadAllText("score.txt");
            }
            else
            {
                File.Create("score.txt");
            }          
            //产生所以方阵和所有方块
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    PictureBox pic = new PictureBox();
                    pic.Top = top + i * size+i;//上边距
                    pic.Left = left + j * size+j;//左边距
                    pic.Visible = false;//初始化可视性为false
                    pic.Enabled = true;//启用
                    pic.Width = size;//宽度
                    pic.Height = size;//高度
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;//图片全铺
                    pictureBox[i, j] = pic;//加入方格组中
                    this.Controls.Add(pic);//加入控件
                }
            }
            random_creat();//生成两个方块
            random_creat();
        }
        private void Game_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = this.CreateGraphics();//画图对象
            Pen pen = new Pen(Color.Black, 2);//笔对象 
            Point point1 = new Point(left, top);
            Point point2 = new Point(left, top + n * size + n + 1);
            Point point3 = new Point(left + n * size + n + 1, top);
            for (int i = 0; i <= n; ++i)
            {                
                g.DrawLine(pen, point1, point2);   
                point1.X += size+1;
                point2.X += size+1;      
            }
            point1 = new Point(left, top);
            for (int j = 0; j <= n; ++j)
            {
                g.DrawLine(pen, point1, point3);
                point1.Y += size+1;
                point3.Y += size+1;
            }
        }
    }
}

