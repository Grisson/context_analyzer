using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Work
{
    //��ԭ
    public class Primitive
    {
        public string Path { get; set; }
        public Dictionary<int, Primitive> ALLPRIMITIVES = new Dictionary<int, Primitive>();
        public Dictionary<string, int> PRIMITIVESID = new Dictionary<string, int>();

        public Primitive()
        {

        }

        public void loadGlossary()
        {
            StreamReader read = null;
            string strLine = "";
            try
            {
                FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read);
                read = new StreamReader(fs, Encoding.GetEncoding("GB2312"));
                read.BaseStream.Seek(0, SeekOrigin.Begin);
                strLine = read.ReadLine();
                //strLine = read.ReadLine();
                strLine = strLine.Trim().ToString();
                int i = 0;
                while (strLine != null)
                {
                    #region test
                    i++;
                    if (i == 999)
                    {
                        string tp5 = "123";
                    }
                    #endregion test
                    //strLine = read.ReadLine();
                    #region regexStr
                    string matchStr = @"\w+\s\s+\d";
                    Regex r = new Regex(matchStr);
                    Match m = r.Match(strLine, 0, strLine.Length);
                    string tpline = strLine;
                    string tp = null;
                    while (m.Success)
                    {
                        tp = m.Value.ToString();
                        tp = tp.Remove(tp.Length - 1);
                        string tp2 = tp.Trim().ToString() + " ";
                        tpline = tpline.Replace(tp, tp2);
                        m = m.NextMatch();
                    }

                    strLine = tpline;

                    string matchStrT = @"\w+\s+";
                    Regex rT = new Regex(matchStrT);
                    Match mT = rT.Match(strLine, 0, strLine.Length);
                    string tplineT = strLine;
                    string tpT = null;
                    while (mT.Success)
                    {
                        tpT = mT.Value.ToString();
                        string tp2T = tpT.Trim().ToString() + " ";
                        tplineT = tplineT.Replace(tpT, tp2T);
                        mT = mT.NextMatch();
                    }
                    strLine = tplineT;
                    #endregion regexStr
                    Log.Write2File("--" + strLine);
                    string[] strs = strLine.Split(' ');
                    int id = int.Parse(strs[0]);
                    string[] words = strs[1].Split('|');
                    string english = words[0];
                    string chinese = strs[1].Split('|')[1];
                    int parentid = int.Parse(strs[2]);
                    //ALLPRIMITIVES.Add(id, new Primitive(id, english, parentid));
                    ALLPRIMITIVES.Add(id, new Primitive(id, english, chinese, parentid));
                    if (!PRIMITIVESID.ContainsKey(chinese))
                        PRIMITIVESID.Add(chinese, id);
                    if (!PRIMITIVESID.ContainsKey(english))
                        PRIMITIVESID.Add(english, id);
                    strLine = read.ReadLine();
                    if (strLine != null)
                    {
                        strLine = strLine.Trim().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write2File("-YiYuan fail--" + ex.Message);
            }
            finally
            {
                try
                {
                    read.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }

        //˽�б����Ķ���
        private int id;
        private string primitiveEnglish;
        private string primitiveChinese;
        private int parentid;
        //���캯�� ��ԭ��ID,��ԭ����ID��
        public Primitive(int id, string primitiveEnglish, string primitiveChinese, int parentid)
        {
            this.id = id;
            this.primitiveEnglish = primitiveEnglish;
            this.primitiveChinese = primitiveChinese;
            this.parentid = parentid;
        }
        //��ȡ��ԭ
        public string getPrimitiveChinese()
        {
            return primitiveChinese;
        }
        public string getprimitiveEnglish()
        {
            return primitiveEnglish;
        }
        //ID��־
        public int getId()
        {
            return id;
        }
        //��ȡ��ID
        public int getParentId()
        {
            return parentid;
        }
        //�Ƿ��Ǹ��ڵ�
        public bool isTop()
        {
            return id == parentid;
        }
        //��ȡһ����ԭ�����и���ԭ��ֱ������λ��
        public List<int> getparents(string primitive)
        {
            List<int> list = new List<int>();
            //��ȡ�����ԭ��ID
            try
            {
                int id = PRIMITIVESID[primitive];
                //���������ԭ��ID
                if (id >= 0)
                {
                    list.Add(id);//������ԭ��ID--���仰˵��������������ԭID����
                    Primitive parent = ALLPRIMITIVES[id];//��ȡ��ԭ�ĸ�����ԭ
                    while (!parent.isTop())//�ж��Ƿ�Ϊ����Ľڵ�
                    {
                        list.Add(parent.getParentId());
                        parent = ALLPRIMITIVES[parent.getParentId()];

                    }
                }
            }
            catch (Exception ex)
            {
            }

            //�����𼶴�����Ľڵ��ID����Ҷ�ӽڵ㵽���ڵ㡣
            return list;
        }
        //�ж��Ƿ�Ϊ��ԭ
        public bool isPrimitive(string primitive)
        {
            return PRIMITIVESID.ContainsKey(primitive);
        }
    }
}
