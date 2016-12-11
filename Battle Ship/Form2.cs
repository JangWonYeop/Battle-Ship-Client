using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace Battle_Ship
{
    public partial class Form2 : Form
    {
        TcpClient client;
        NetworkStream stream;
        int settingSequence = 0;
        int blockType = 0; // 가로 = 1, 세로 = 2

        // 그리드뷰 배열
        int[,] data = new int[6, 6];

        // 배치 과정에서 선택한 셀에 이미 데이터가 있을경우 에러
        private bool isAvailable(int x, int y, int seq, int bType)
        {
            bool available = true;

            // 데이터 배열과 비교
            // 6칸짜리
            if(seq == 1)
            {
                // 가로모양
                if(bType == 1)
                {
                    for(int i=x; i<x+3; i++)
                    {
                        for(int j= y; j<y+2; j++)
                        {
                            if(data[i, j] != 0)
                            {
                                available = false;
                                break;
                            }
                        }
                    }
                }
                // 세로모양
                else if(bType == 2)
                {
                    for (int i = x; i < x + 2; i++)
                    {
                        for (int j = y; j < y + 3; j++)
                        {
                            if (data[i, j] != 0)
                            {
                                available = false;
                                break;
                            }
                        }
                    }
                }
            }
            // 2칸짜리
            else if(seq == 2)
            {
                // 가로모양
                if (bType == 1)
                {
                    if(data[x, y] != 0 || data[x+1, y] != 0)
                    {
                        available = false;
                    }
                }
                // 세로모양
                else if (bType == 2)
                {
                    if (data[x, y] != 0 || data[x, y+1] != 0)
                    {
                        available = false;
                    }
                }
            }

            return available;
        }

        // 그리드뷰 초기화
        private void gridViewInitialize()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    dataGridView1[i, j].Style.BackColor = System.Drawing.Color.Gray;
                    dataGridView1[i, j].Value = null;
                }
            }

        }

        // data값 그리드뷰에 대입
        private void matchingDataWithGridview()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (data[i, j] != 0)
                    {
                        dataGridView1[i, j].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[i, j].Value = data[i, j].ToString();
                    }
                }
            }
        }
              
        public Form2()
        {
            InitializeComponent();
        }
        
        public Form2(TcpClient client, NetworkStream stream)
        {
            this.client = client;
            this.stream = stream;

            InitializeComponent();

            this.Text = "배치 결정 (1/3)";
            label1.Text = "배치 유형 선택 후 맵에 위치시키세요. (1/3)";

            for (int i = 0; i < 5; i++)
            {
                dataGridView1.Rows.Add();
            }
            for (int i = 0; i < 6; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                row.Height = 50;
            }

            // 그리드뷰 색상
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    dataGridView1[i, j].Style.BackColor = System.Drawing.Color.Gray;
                }
            }

            // data 0으로 초기화
            for (int i=0; i<6; i++)
            {
                for(int j=0; j<6; j++)
                {
                    data[i, j] = 0;
                }
            }

            // 결정버튼 비활성화
            button4.Enabled = false;

            // 배치 보내기

        }

        // 연결 테스트 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            // 쓰기
            String message = "Message from client2";
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent: {0}", message);

            // 읽기
            data = new Byte[256];
            String responseData = String.Empty;
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);

            // 출력
            MessageBox.Show(responseData);
        }

        // 블록 유형 가로 선택
        private void button2_Click(object sender, EventArgs e)
        {
            blockType = 1;
            dataGridView1.Visible = true;
        }

        // 블록 유형 세로 선택
        private void button3_Click(object sender, EventArgs e)
        {
            blockType = 2;
            dataGridView1.Visible = true;
        }

        // 그리드 클릭 이벤트 처리
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 선택한 셀 위치 얻기
            int x = dataGridView1.CurrentCell.ColumnIndex;
            int y = dataGridView1.CurrentCell.RowIndex;

             // 세팅과정 2칸짜리
            if(settingSequence == 0)
            {
                // 그리드뷰 초기화
                gridViewInitialize();

                // data값 그리드뷰에 대입
                matchingDataWithGridview();

                // 가로 모양일때
                if (blockType == 1) {
                    if(x == 5)
                    {
                        MessageBox.Show("배치 가능 범위를 벗어났습니다.");
                        return;
                    }

                    // 중복 처리
                    if (isAvailable(x, y, settingSequence, blockType) == false)
                    {
                        MessageBox.Show("중복되는 위치에 배치할 수 없습니다.");
                        return;
                    }
                    else
                    {
                        dataGridView1[x, y].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x+1, y].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x, y].Value = "1";
                        dataGridView1[x + 1, y].Value = "1";

                        button4.Enabled = true;
                    }
                }
                // 세로 모양일떄
                else if(blockType == 2)
                {
                    if(y == 5)
                    {
                        MessageBox.Show("해당 위치에 배치할 수 없습니다.");
                        return;
                    }

                    // 중복 처리
                    if (isAvailable(x, y, settingSequence, blockType) == false)
                    {
                        MessageBox.Show("중복되는 위치에 배치할 수 없습니다.");
                        return;
                    }
                    else
                    {
                        dataGridView1[x, y].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x, y+1].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x, y].Value = "1";
                        dataGridView1[x, y+1].Value = "1";

                        button4.Enabled = true;
                    }
                }

                // 블록 타입을 선택하지 않았을 경우
                else
                {
                    MessageBox.Show("모양을 선택하세요.");
                    return;
                }
            }

            // 6칸짜리
            else if(settingSequence == 1)
            {
                // 그리드뷰 초기화
                gridViewInitialize();

                // data값 그리드뷰에 대입
                matchingDataWithGridview();

                // 가로
                if (blockType == 1)
                {
                    if(x > 3 || y > 4)
                    {
                        MessageBox.Show("해당 위치에 배치할 수 없습니다.");
                        return;
                    }

                    // 중복 처리
                    if (isAvailable(x, y, settingSequence, blockType) == false)
                    {
                        MessageBox.Show("중복되는 위치에 배치할 수 없습니다.");
                        return;
                    }
                    else
                    {
                        for(int i=x; i<x+3; i++)
                        {
                            for(int j= y; j<y+2; j++)
                            {
                                dataGridView1[i, j].Style.BackColor = System.Drawing.Color.Blue;
                                dataGridView1[i, j].Value = "2";
                            }
                        }

                        button4.Enabled = true;
                    }
                }
                // 세로
                else if (blockType == 2)
                {
                    if (x > 4 || y > 3)
                    {
                        MessageBox.Show("해당 위치에 배치할 수 없습니다.");
                        return;
                    }

                    // 중복 처리
                    if (isAvailable(x, y, settingSequence, blockType) == false)
                    {
                        MessageBox.Show("중복되는 위치에 배치할 수 없습니다.");
                        return;
                    }
                    else
                    {
                        for (int i = x; i < x + 2; i++)
                        {
                            for (int j = y; j < y + 3; j++)
                            {
                                dataGridView1[i, j].Style.BackColor = System.Drawing.Color.Blue;
                                dataGridView1[i, j].Value = "2";
                            }
                        }

                        button4.Enabled = true;
                    }
                }

                // 블록 타입을 선택하지 않았을 경우
                else
                {
                    MessageBox.Show("모양을 선택하세요.");
                    return;
                }
            }

            // 2칸짜리
            else if(settingSequence == 2)
            {
                // 그리드뷰 초기화
                gridViewInitialize();

                // data값 그리드뷰에 대입
                matchingDataWithGridview();

                // 가로 모양일때
                if (blockType == 1)
                {
                    if (x == 5)
                    {
                        MessageBox.Show("해당 위치에 배치할 수 없습니다.");
                        return;
                    }

                    // 중복 처리
                    if (isAvailable(x, y, settingSequence, blockType) == false)
                    {
                        MessageBox.Show("중복되는 위치에 배치할 수 없습니다.");
                        return;
                    }
                    else
                    {
                        dataGridView1[x, y].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x + 1, y].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x, y].Value = "3";
                        dataGridView1[x + 1, y].Value = "3";

                        button4.Enabled = true;
                    }
                }
                // 세로 모양일떄
                else if (blockType == 2)
                {
                    if (y == 5)
                    {
                        MessageBox.Show("해당 위치에 배치할 수 없습니다.");
                        return;
                    }

                    // 중복 처리
                    if (isAvailable(x, y, settingSequence, blockType) == false)
                    {
                        MessageBox.Show("중복되는 위치에 배치할 수 없습니다.");
                        return;
                    }
                    else
                    {
                        dataGridView1[x, y].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x, y + 1].Style.BackColor = System.Drawing.Color.Blue;
                        dataGridView1[x, y].Value = "3";
                        dataGridView1[x, y + 1].Value = "3";

                        button4.Enabled = true;
                    }
                }

                // 블록 타입을 선택하지 않았을 경우
                else
                {
                    MessageBox.Show("모양을 선택하세요.");
                    return;
                }
            }

            // 그 외
            else
            {

            }
        }

        // 결정버튼
        private void button4_Click(object sender, EventArgs e)
        {
            // 현재 그리드뷰 데이터 저장
            for(int i=0; i<6; i++)
            {
                for(int j=0; j<6; j++)
                {
                    if((String)dataGridView1[i, j].Value == "1")
                    {
                        data[i, j] = 1;
                    }
                    else if ((String)dataGridView1[i, j].Value == "2")
                    {
                        data[i, j] = 2;
                    }
                    else if ((String)dataGridView1[i, j].Value == "3")
                    {
                        data[i, j] = 3;
                    }
                }
            }

            settingSequence++;
            blockType = 0;

            // 인터페이스 변경
            if (settingSequence == 1)
            {
                this.Text = "배치 결정 (2/3)";
                label1.Text = "배치 유형 선택 후 맵에 위치시키세요. (2/3)";
                label2.Text = "6칸 짜리";
                button2.Size = new Size(60, 40);
                button3.Size = new Size(40, 60);
            }
            else if(settingSequence == 2)
            {
                this.Text = "배치 결정 (3/3)";
                label1.Text = "배치 유형 선택 후 맵에 위치시키세요. (3/3)";
                label2.Text = "2칸 짜리";
                button2.Size = new Size(40, 20);
                button3.Size = new Size(20, 40);
            }

            // 최종결정
            else if(settingSequence == 3)
            {
                this.Text = "게임 시작 대기중...";
                label1.Text = "배치를 완료했습니다. \n게임 시작 대기중...";
                label2.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                label5.Visible = false;
            }

            button4.Enabled = false;
        }

    }
}
