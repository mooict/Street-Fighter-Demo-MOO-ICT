using System.Drawing.Imaging;


namespace Street_Fighter_Demo_MOO_ICT
{
    public partial class Form1 : Form
    {
        Image player;
        Image background;
        Image drum;
        Image fireball;

        int playerX = 0;
        int playerY = 300;

        int drumX = 450;
        int drumY = 335;

        int fireballX;
        int fireballY;

        int drumMoveTime = 0;
        int actionStrength = 0;
        int endFrame = 0;
        int backgroundPosition = 0;
        int totalFrame = 0;
        int bg_number = 0;

        float num;

        bool goLeft, goRight;
        bool directionPressed;
        bool playingAction;
        bool shotFireBall;

        List<string> background_images = new List<string>();


        public Form1()
        {
            InitializeComponent();
            SetUpForm();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && !directionPressed)
            {
                MovePlayerAnimation("left");
            }
            if (e.KeyCode == Keys.Right && !directionPressed)
            {
                MovePlayerAnimation("right");
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X)
            {
                if (bg_number < background_images.Count - 1)
                {
                    bg_number++;
                }
                else
                {
                    bg_number = 0;
                }
                background = Image.FromFile(background_images[bg_number]);
                backgroundPosition = 0;
                drumMoveTime = 0;
                drumX = 300;
            }

            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
            {
                goLeft = false;
                goRight = false;
                directionPressed = false;
                ResetPlayer();
            }

            if (e.KeyCode == Keys.A && !playingAction && !goLeft && !goRight)
            {
                SetPlayerAction("punch2.gif", 2);
            }
            if (e.KeyCode == Keys.S && !playingAction && !goLeft && !goRight)
            {
                SetPlayerAction("punch1.gif", 5);
            }
            if (e.KeyCode == Keys.D && !playingAction && !goLeft && !goRight && !shotFireBall)
            {
                SetPlayerAction("fireball.gif", 30);
            }


        }

        private void FormPaintEvent(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(background, new Point(backgroundPosition, 0));
            e.Graphics.DrawImage(player, new Point(playerX, playerY));
            e.Graphics.DrawImage(drum, new Point(drumX, drumY));

            if (shotFireBall)
            {
                e.Graphics.DrawImage(fireball, new Point(fireballX, fireballY));
            }

        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            this.Invalidate();
            ImageAnimator.UpdateFrames();
            MovePlayerandBackground();
            CheckPunchHit();

            if (playingAction)
            {
                if (num < totalFrame)
                {
                    num += 0.5f;
                }
            }
            if (num == totalFrame)
            {
                ResetPlayer();
            }

            // fireball instructions for the time. 

            if (shotFireBall)
            {
                fireballX += 10;
                CheckFireballHit();
            }

            if (fireballX > this.ClientSize.Width)
            {
                shotFireBall = false;
            }

            if (!shotFireBall && num > endFrame && drumMoveTime == 0 && actionStrength == 30)
            {
                ShootFireball();
            }

            if (drumMoveTime > 0)
            {
                drumMoveTime--;
                drumX += 10;
                drum = Image.FromFile("hitdrum.png");
            }
            else
            {
                drum = Image.FromFile("drum.png");
                drumMoveTime = 0;
            }

            if (drumX > this.ClientSize.Width)
            {
                drumMoveTime = 0;
                drumX = 300;
            }


        }

        private void SetUpForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            background_images = Directory.GetFiles("background", "*.jpg").ToList();
            background = Image.FromFile(background_images[bg_number]);
            player = Image.FromFile("standing.gif");
            drum = Image.FromFile("drum.png");
            SetUpAnimation();

        }

        private void SetUpAnimation()
        {
            ImageAnimator.Animate(player, this.OnFrameChangedHandler);
            FrameDimension dimentions = new FrameDimension(player.FrameDimensionsList[0]);
            totalFrame = player.GetFrameCount(dimentions);
            endFrame = totalFrame - 3;
        }

        private void OnFrameChangedHandler(object? sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void MovePlayerandBackground()
        {
            if (goLeft)
            {
                if (playerX > 0)
                {
                    playerX -= 6;
                }

                if (backgroundPosition < 0 && playerX < 100)
                {
                    backgroundPosition += 5;
                    drumX += 5;
                }

            }
            if (goRight)
            {
                if (playerX + player.Width < this.ClientSize.Width)
                {
                    playerX += 6;
                }

                if (backgroundPosition + background.Width > this.ClientSize.Width + 5 && playerX > 650)
                {
                    backgroundPosition -= 5;
                    drumX -= 5;
                }

            }
        }

        private void MovePlayerAnimation(string direction)
        {
            if (direction == "left")
            {
                goLeft = true;
                player = Image.FromFile("backwards.gif");
            }
            if (direction == "right")
            {
                goRight = true;
                player = Image.FromFile("forwards.gif");
            }

            directionPressed = true;
            playingAction = false;
            SetUpAnimation();
        }

        private void ResetPlayer()
        {
            player = Image.FromFile("standing.gif");
            SetUpAnimation();
            num = 0;
            playingAction = false;


        }

        private void SetPlayerAction(string animation, int strength)
        {
            player = Image.FromFile(animation);
            actionStrength = strength;
            SetUpAnimation();
            playingAction = true;
        }

        private void ShootFireball()
        {
            fireball = Image.FromFile("FireBallFinal.gif");
            ImageAnimator.Animate(fireball, this.OnFrameChangedHandler);
            fireballX = playerX + player.Width - 50;
            fireballY = playerY - 33;
            shotFireBall = true;
        }

        private void CheckPunchHit()
        {
            bool collision = DetectCollision(playerX, playerY, player.Width, player.Height, drumX, drumY, drum.Width, drum.Height);

            if (collision && playingAction && num > endFrame)
            {
                drumMoveTime = actionStrength;
            }
        }

        private void CheckFireballHit()
        {
            bool collision = DetectCollision(fireballX, fireballY, fireball.Width, fireball.Height, drumX, drumY, drum.Width, drum.Height);

            if (collision)
            {
                drumMoveTime = actionStrength;
                shotFireBall = false;
            }

        }

        private bool DetectCollision(int object1X, int object1Y, int object1Width, int object1Height, int object2X, int object2Y, int object2Width, int object2Height)
        {
            if (object1X + object1Width <= object2X || object1X >= object2X + object2Width || object1Y + object1Height <= object2Y || object1Y >= object2Y + object2Height)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}