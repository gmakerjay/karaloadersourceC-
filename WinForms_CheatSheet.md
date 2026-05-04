# 📚 คู่มือคำสั่ง WinForms C# พื้นฐาน (WinForms Cheat Sheet)

คู่มือฉบับนี้รวบรวม "ชิ้นส่วน (Controls)", "คุณสมบัติ (Properties)", และ "เหตุการณ์ (Events)" ที่มีในไลบรารี `System.Windows.Forms` เอาไว้ให้คุณดูเป็นไอเดียเวลาจะสร้างโปรแกรมหน้าต่าง (GUI) ครับ

## 📦 ไลบรารี (Libraries) ที่จำเป็นต้องประกาศไว้บนสุดของไฟล์เสมอ
เวลาจะเขียน WinForms ด้วยโค้ดมือ (Code-behind) ห้ามลืม 2 บรรทัดนี้เด็ดขาด:
```csharp
using System.Windows.Forms; // สำหรับสร้างชิ้นส่วนต่างๆ (ปุ่ม, ข้อความ, หน้าต่าง)
using System.Drawing;       // สำหรับกำหนดสี (Color), ขนาด (Size), พิกัด (Point), และฟอนต์ (Font)
```

---

## 🖥️ 1. หน้าต่างโปรแกรมหลัก (`Form`)
ตัวแม่ของทุกอย่าง ทำหน้าที่เป็นกรอบหน้าต่างโปรแกรม
```csharp
Form myForm = new Form();
myForm.Text = "ชื่อบนหัวโปรแกรม";
myForm.Size = new Size(800, 600); // ความกว้าง, ความสูง
myForm.BackColor = Color.White; // สีพื้นหลัง
myForm.StartPosition = FormStartPosition.CenterScreen; // เปิดมาอยู่กลางจอ
myForm.FormBorderStyle = FormBorderStyle.FixedSingle; // ห้ามยืดหดหน้าต่าง
```
**Events ที่ใช้บ่อย:**
- `myForm.Load += (s, e) => { ... };` (ตอนเปิดโปรแกรมขึ้นมาครั้งแรก)
- `myForm.FormClosing += (s, e) => { ... };` (ตอนที่ผู้ใช้กดกากบาทปิดโปรแกรม)

---

## 🔲 2. ปุ่มกด (`Button`)
ปุ่มสำหรับคลิกสั่งงาน
```csharp
Button btn = new Button();
btn.Text = "ตกลง";
btn.Size = new Size(100, 40);
btn.Location = new Point(50, 50); // พิกัด X, Y นับจากมุมซ้ายบน
btn.BackColor = Color.Blue;
btn.ForeColor = Color.White; // สีตัวหนังสือ
btn.Cursor = Cursors.Hand; // เอาเมาส์ชี้แล้วเป็นรูปนิ้ว
btn.FlatStyle = FlatStyle.Flat; // ทำให้ปุ่มแบนๆ ดูทันสมัย
```
**Events ที่ใช้บ่อย:**
- `btn.Click += (s, e) => { MessageBox.Show("โดนคลิกแล้ว!"); };`
- `btn.MouseEnter += (s, e) => { btn.BackColor = Color.Red; };` (เมาส์ชี้เปลี่ยนสี)
- `btn.MouseLeave += (s, e) => { btn.BackColor = Color.Blue; };` (เมาส์ออกกลับสีเดิม)

---

## 📝 3. ข้อความ (`Label`)
เอาไว้แสดงข้อความเฉยๆ ไม่ให้พิมพ์แก้
```csharp
Label lbl = new Label();
lbl.Text = "กรุณากรอกชื่อของคุณ";
lbl.Font = new Font("Arial", 14F, FontStyle.Bold); // เปลี่ยนฟอนต์ ขนาด และทำตัวหนา
lbl.ForeColor = Color.Red;
lbl.AutoSize = true; // ยืดขนาดกล่องตามความยาวข้อความอัตโนมัติ
lbl.Location = new Point(20, 20);
```

---

## ⌨️ 4. ช่องกรอกข้อความ (`TextBox`)
ให้ผู้ใช้พิมพ์ข้อความลงไปได้
```csharp
TextBox txt = new TextBox();
txt.Size = new Size(200, 30);
txt.Location = new Point(20, 50);
txt.PasswordChar = '*'; // ใส่ไว้ถ้าเป็นช่องกรอกรหัสผ่าน
txt.ReadOnly = false; // ถ้าเป็น true จะพิมพ์แก้ไม่ได้
txt.Multiline = true; // อนุญาตให้พิมพ์หลายบรรทัดได้ (เหมือนช่อง Comment)
```
**Events ที่ใช้บ่อย:**
- `txt.TextChanged += (s, e) => { ... };` (ทำงานทุกครั้งที่พิมพ์ 1 ตัวอักษร)
- `txt.KeyDown += (s, e) => { if(e.KeyCode == Keys.Enter) { ... } };` (ดักจับตอนกดปุ่ม Enter)

---

## ☑️ 5. ช่องติ๊กถูก (`CheckBox`)
สวิตช์เปิด/ปิด หรือยอมรับเงื่อนไข
```csharp
CheckBox chk = new CheckBox();
chk.Text = "ฉันยอมรับเงื่อนไข";
chk.Checked = true; // ตั้งให้ติ๊กถูกมาตั้งแต่เริ่ม
chk.Location = new Point(20, 100);
```
**Events ที่ใช้บ่อย:**
- `chk.CheckedChanged += (s, e) => { ... };` (ทำงานตอนถูกติ๊กเข้า/ออก)

---

## 🗂️ 6. กล่องตัวเลือก Dropdown (`ComboBox`)
ให้เลือกรายการที่กำหนดไว้
```csharp
ComboBox cbo = new ComboBox();
cbo.Location = new Point(20, 150);
cbo.DropDownStyle = ComboBoxStyle.DropDownList; // ห้ามพิมพ์เอง บังคับเลือกจากรายการ
cbo.Items.Add("ใช้งาน 30 วัน");
cbo.Items.Add("ใช้งาน 90 วัน");
cbo.SelectedIndex = 0; // เลือกตัวแรกเป็นค่าเริ่มต้น
```
**Events ที่ใช้บ่อย:**
- `cbo.SelectedIndexChanged += (s, e) => { ... };` (ทำงานตอนที่เปลี่ยนตัวเลือก)

---

## 🟩 7. หลอดความคืบหน้า (`ProgressBar`)
ใช้แสดงสถานะการโหลด/ดาวน์โหลด
```csharp
ProgressBar pb = new ProgressBar();
pb.Size = new Size(300, 20);
pb.Location = new Point(20, 200);
pb.Minimum = 0;
pb.Maximum = 100;
pb.Value = 50; // หลอดจะวิ่งไปที่ 50%
pb.Style = ProgressBarStyle.Continuous; // ให้หลอดวิ่งเนียนๆ
// ถ้าเป็น ProgressBarStyle.Marquee หลอดจะวิ่งวนไปเรื่อยๆ (เหมาะกับตอนไม่รู้เวลาเสร็จแน่ชัด)
```

---

## 🖼️ 8. แผงจัดกลุ่ม (`Panel`)
ใช้ตีกรอบสี่เหลี่ยม เพื่อเอาของอื่นๆ (ปุ่ม, ข้อความ) เข้าไปใส่รวมกันเป็นกลุ่ม
```csharp
Panel pnl = new Panel();
pnl.Size = new Size(400, 300);
pnl.BackColor = Color.LightGray;
pnl.BorderStyle = BorderStyle.FixedSingle; // ใส่ขอบให้ Panel

// การเอาของใส่ Panel
pnl.Controls.Add(btn);
pnl.Controls.Add(lbl);
```

---

## 📢 การประกอบร่าง!
จำไว้เสมอว่า สร้างเสร็จแล้ว ต้องเอาไป "แปะหน้าจอ" เสมอ ไม่งั้นจะไม่โผล่มาให้เห็น

```csharp
// 1. จองชื่อ
Button myBtn;

// 2. สร้างและตั้งค่า
myBtn = new Button();
myBtn.Text = "กดสิ";

// 3. แปะลงหน้าต่างโปรแกรม (หรือแปะลง Panel)
this.Controls.Add(myBtn); 
```

---

## 🚀 ตัวอย่างโค้ดโปรแกรมฉบับสมบูรณ์ (เอาไปรันได้เลย)
นี่คือตัวอย่างการสร้างโปรแกรมทักทายง่ายๆ ที่มี 1 หน้าต่าง, 1 ช่องกรอกชื่อ, 1 ปุ่มกด, และ 1 ข้อความแสดงผล

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyFirstApp
{
    // สร้างหน้าต่างหลัก โดยสืบทอดความสามารถมาจาก Form
    public class HelloForm : Form
    {
        private TextBox txtName;
        private Button btnGreet;
        private Label lblResult;

        public HelloForm()
        {
            // ตั้งค่าหน้าต่าง
            this.Text = "โปรแกรมทักทาย";
            this.Size = new Size(300, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 1. สร้างช่องกรอกชื่อ
            txtName = new TextBox();
            txtName.Location = new Point(50, 30);
            txtName.Size = new Size(180, 30);
            this.Controls.Add(txtName);

            // 2. สร้างปุ่มกด
            btnGreet = new Button();
            btnGreet.Text = "กดเพื่อทักทาย";
            btnGreet.Location = new Point(50, 70);
            btnGreet.Size = new Size(180, 40);
            btnGreet.BackColor = Color.LightBlue;
            
            // ใส่ Event เมื่อปุ่มโดนคลิก
            btnGreet.Click += (s, e) => 
            {
                lblResult.Text = "สวัสดีคุณ: " + txtName.Text;
            };
            this.Controls.Add(btnGreet);

            // 3. สร้างข้อความแสดงผล
            lblResult = new Label();
            lblResult.Text = "...รอการกดปุ่ม...";
            lblResult.Location = new Point(50, 120);
            lblResult.AutoSize = true;
            this.Controls.Add(lblResult);
        }
    }

    // จุดเริ่มต้นโปรแกรม
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new HelloForm()); // สั่งรันหน้าต่างที่เราสร้างไว้
        }
    }
}
```
