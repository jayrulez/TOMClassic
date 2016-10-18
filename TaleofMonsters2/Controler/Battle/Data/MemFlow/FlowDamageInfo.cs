using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    internal class FlowDamageInfo : FlowWord
    {
        private HitDamage damage;
        public FlowDamageInfo(HitDamage dam, Point point) 
            : base("", point, 3, "Coral", 0, -10, 0, 2, 30)
        {
            damage = dam;
            word = damage.Value.ToString();
            if (damage.IsCrt)
            {
                size += 6;//��������Ŵ��ƶ��ٶȼ���
                font.Dispose();
                font = new Font("΢���ź�", this.size * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                speedY --;//-4
                word = damage.Value.ToString() + "!";
            }
        }

        internal override void Draw(Graphics g)
        {
            int xOff = position.X-10;

            if (damage.Dtype == DamageTypes.Magic)
            {
                xOff -= 16;
                g.DrawImage(HSIcons.GetIconsByEName("atr"+damage.Element), xOff, position.Y + 8, 18, 18);
                xOff += 16;
                color = PaintTool.GetColorByAttribute(damage.Element);
            }

            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, xOff, position.Y);
            brush.Dispose();
        } 
    }
}
