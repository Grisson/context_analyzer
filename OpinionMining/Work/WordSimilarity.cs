using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Work
{
    //���ƶȼ���
    public class WordSimilarity
    {
        public string Path { get; set; }
        public Dictionary<string, List<Word>> ALLWORDS = new Dictionary<string, List<Word>>();
        // sim(p1,p2)=alpha/(d+alpha)
        private double alpha = 1.6;
        private double beta1 = 0.5;
        private double beta2 = 0.2;
        private double beta3 = 0.17;
        private double beta4 = 0.13;
        //���������ԭ�����ƶ�һ�ɴ���Ϊһ���Ƚ�С�ĳ���. ����ʺ;���ʵ����ƶȣ������������ͬ����Ϊ1������Ϊ0
        private double gamma = 0.2;
        // ����һ�ǿ�ֵ���ֵ�����ƶȶ���Ϊһ���Ƚ�С�ĳ���
        private double delta = 0.2;
        //�����޹���ԭ֮���Ĭ�Ͼ���
        private int DEFAULT_PRIMITIVE_DIS = 20;
        //֪���е��߼�����
        private static String LOGICAL_SYMBOL = ",~^";
        //֪���еĹ�ϵ����, �����������ķ��ţ�˵��Ϊ����Ĺ�ϵ��ԭ
        private String RELATIONAL_SYMBOL = "#%$*+&@?!";
        //֪���е�������ţ���ʣ�������,֪���е���ʶ�����{}��������
        private string SPECIAL_SYMBOL = "{";
        public WordSimilarity()
        {
            //  loadGlossary();
        }

        //��ԭ��
        public Primitive primitive { get; set; }

        //����
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
                int jj = 0;
                while (strLine != null)
                {

                    /* #region regexStr
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
                      #endregion regexStr */
                    string[] strs = System.Text.RegularExpressions.Regex.Split(strLine, @"\s+");
                    string word = strs[0];//��һ���Ǵ���
                    string type = strs[1];//�ڶ���������
                    string related = strs[2];
                    //Log.Write2File("---" + jj.ToString()+  ", " + word + ", " + type + ", " + related + "---");

                    /*   //��Ϊ�ǰ��ո񻮷֣����һ���ֵļӻ�ȥ--���ͣ���Ϊ�����ÿ�����֮����ܴ��ڿո�
                    for (int i = 3; i < strs.Length; i++)
                    {
                        related += (" " + strs[i]);

                    }*/
                    //jj++;
                    //if (jj == 3800)
                    //jj = 0;
                    //��ǰ�������ִ���һ���´�
                    Word w = new Word();
                    w.setWord(word);
                    w.setType(type);
                    w.setRelated(related);

                    parseDetail(related, w);//֪���еĹؼ����������������������ν����������ԭ�ġ�
                    // save this word.


                    addWord(w);
                    //Write2File(outpath, "-success--");
                    // read the next line

                    strLine = read.ReadLine();
                    if (strLine != null)
                    {
                        strLine = strLine.Trim().ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Write2File("-YiXiang fail--" + ex.Message);
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

        //�����������֣��������Ľ������<code>Word word</code>
        public void parseDetail(string related, Word word)
        {
            bool isSpecial = false; // �Ƿ����
            //�����ж��Ƿ���ʣ��������{firstPerson|��,mass|��}
            if (related.StartsWith(SPECIAL_SYMBOL))
            {
                isSpecial = true;//�����
                related = related.Substring(1, related.Length - 2);
            }

            //����,�ֿ� ���ӣ�aValue|����ֵ,attachment|����,#country|����,ProperName|ר
            string[] parts = related.Split(',');
            bool isFirst = true;//�Ƿ�Ϊ��һ������Ҫ��Ϊ�˻�ȡ��һ��ԭ�����õĲ���
            bool isRelational = false;//�Ƿ��ǹ�ϵ��ԭ
            bool isSimbol = false;//�Ƿ������ԭ
            string chinese = null;//�Ƿ�������
            string relationalPrimitiveKey = null;//��ϵ��ԭ��KEYֵ
            string simbolKey = null;//���ŵ�KEY
            for (int i = 0; i < parts.Length; i++)
            {
                if (isSpecial == true)
                {
                    string[] strs = parts[i].Split('|');
                    if (strs.Length > 1)
                    {
                        // |����������ĵ�ֵVALUE
                        word.addStructruralWord(strs[1]);
                    }
                    else
                    {
                        //���û�����Ĳ��֣���ʹ��Ӣ�Ĵ�
                        word.addStructruralWord(strs[0]);
                    }
                    continue;
                }
                //����Ǿ���ʣ��������ſ�ʼ�ͽ�β: (Afghanistan|������)
                if (parts[i].StartsWith("("))
                {
                    //ȥ������ʵ������������ţ�ʣ�µ�parts[i]��ŵ��Ǿ���ʱ���
                    parts[i] = parts[i].Substring(1, parts[i].Length - 1);
                    // parts[i] = parts[i].Replace("\\s+", "");
                }
                //��ϵ��ԭ��֮��Ķ��ǹ�ϵ��ԭ
                if (parts[i].Contains("="))
                {
                    isRelational = true;//��ϵ��ԭ��־TRUE
                    // format: content=fact|����
                    string[] strs = parts[i].Split('=');
                    relationalPrimitiveKey = strs[0];//�Ⱥ�ǰ�����KEY
                    string[] values = strs[1].Split('|');
                    if (values.Length > 1)
                    {  //�Ⱥź����|����������ĵ�ֵVALUE
                        if (relationalPrimitiveKey != string.Empty)
                        {
                            word.addRelationalPrimitive(relationalPrimitiveKey, values[1]);//��ӹ�ϵ��ԭ
                        }
                    }
                    //������һ��ѭ��
                    continue;
                }
                //��ȡ���Ĳ���chinese
                string[] strss = parts[i].Split('|');
                // �������Ĳ��ֵĴ���,�������û�����Ľ���
                if (strss.Length > 1)
                {
                    //��ȡ|��������Ĳ���
                    chinese = strss[1];
                }
                //�������Ĳ��֣����п��ܴ����� �����
                if (chinese != null && (chinese.EndsWith(")") || chinese.EndsWith("}")))
                {
                    //С���0��ʼ����Ϊ��(Europe|ŷ��)
                    chinese = chinese.Substring(0, chinese.Length - 1);

                }
                //��ʼ�ĵ�һ���ַ���ȷ�� �����ԭ�����
                int type = getPrimitiveType(strss[0]);
                //����������ԭ
                if (type == 0)
                {
                    // ֮ǰ��һ����ϵ��ԭ������ľͶ����뵽��ϵ��ԭ�������С�
                    if (isRelational)
                    {
                        word.addRelationalPrimitive(relationalPrimitiveKey, chinese);
                        continue;
                    }
                    // ֮ǰ��һ���Ƿ�����ԭ������ľͶ����뵽������ԭ�С�
                    if (isSimbol)
                    {
                        word.addRelationSimbolPrimitive(simbolKey, chinese);
                        continue;
                    }
                    //����ǵ�һ�������뵽������ԭ�б����򣬼��뵽������ԭ�б�
                    if (isFirst)
                    {
                        word.setFirstPrimitive(chinese);
                        isFirst = false;
                        continue;
                    }
                    else
                    {
                        word.addOtherPrimitive(chinese);
                        continue;
                    }

                }
                // ��ϵ���ű�
                if (type == 1)
                {
                    isSimbol = true;
                    isRelational = false;
                    //ȡ��ǰ��ĵ�һ�����ţ�����ϵ��ԭ�ķ���!@#$%*��
                    simbolKey = (strss[0].ToCharArray()[0]).ToString();
                    //��ӵ���ϵ������ԭ��������
                    word.addRelationSimbolPrimitive(simbolKey, chinese);
                    continue;
                }

            }

        }
        //��Ӣ�Ĳ���ȷ�������ԭ�����
        //return һ������������������ֵΪ0,1
        public int getPrimitiveType(string str)
        {
            string first = (str.ToCharArray()[0]).ToString();
            if (RELATIONAL_SYMBOL.Contains(first))
            {
                return 1;//������ԭ
            }
            /* ����ȥ����ʵ��жϣ�����жϸ���parseDetail���ж�
            if (SPECIAL_SYMBOL.Contains(first))
            {
                return 2;//���
            }*/
            return 0;//������ԭ
        }


        //����������������ƶ�
        //������������֮������ƶȣ�ȡ�����������������֮������ֵ��Ϊ������������ƶ�
        public double simWord(string word1, string word2)
        {
            if (ALLWORDS.ContainsKey(word1) && ALLWORDS.ContainsKey(word2))
            {
                List<Word> list1 = ALLWORDS[word1];//ͬһ������ж����У��ʹ��ڶ��ٸ�����(����)
                List<Word> list2 = ALLWORDS[word2];
                double max = 0;
                Word[] lst1 = new Word[list1.Count];
                list1.CopyTo(lst1, 0);
                Word[] lst2 = new Word[list2.Count];
                list2.CopyTo(lst2, 0);
                for (int i = 0; i < lst1.Length; i++)
                {
                    Word w1 = lst1[i];
                    for (int j = 0; j < lst2.Length; j++)
                    {
                        Word w2 = lst2[j];
                        double sim = simWord(w1, w2);
                        max = (sim > max) ? sim : max;
                    }
                }
                return max;
            }
            return 0;

        }
        //����2������
        public double simWord(Word w1, Word w2)
        {
            // ��ʺ�ʵ�ʵ����ƶ�Ϊ��
            if (w1.isStructruralWord() != w2.isStructruralWord())
            {
                return 0;
            }
            //���
            //������ʸ��������á�{�䷨��ԭ}����{��ϵ��ԭ}�������ַ�ʽ�������������ԣ���ʸ�������ƶȼ���ǳ��򵥣�ֻ��Ҫ�������Ӧ�ľ䷨��ԭ���ϵ��ԭ֮������ƶȼ��ɡ�
            if (w1.isStructruralWord() && w2.isStructruralWord())
            {
                List<string> list1 = w1.getStructruralWords();
                List<string> list2 = w2.getStructruralWords();
                return simList(list1, list2);
            }
            //ʵ��
            if (!w1.isStructruralWord() && !w2.isStructruralWord())
            {
                // ʵ�ʵ����ƶȷ�Ϊ4������
                // ������ԭ���ƶ�
                string firstPrimitive1 = w1.getFirstPrimitive();
                string firstPrimitive2 = w2.getFirstPrimitive();
                double sim1 = simPrimitive(primitive, firstPrimitive1, firstPrimitive2);
                // ����������ԭ���ƶ�
                List<string> list1 = w1.getOtherPrimitives();
                List<string> list2 = w2.getOtherPrimitives();
                double sim2 = simList(list1, list2);
                // ��ϵ��ԭ���ƶ�
                Dictionary<string, List<string>> dic1 = w1.getRelationalPrimitives();
                Dictionary<string, List<string>> dic2 = w2.getRelationalPrimitives();
                double sim3 = simDictionary(dic1, dic2);
                // ��ϵ�������ƶ�
                dic1 = w1.getRelationSimbolPrimitives();
                dic2 = w2.getRelationSimbolPrimitives();
                double sim4 = simDictionary(dic1, dic2);

                double product = sim1;
                double sum = beta1 * product;
                product *= sim2;
                sum += beta2 * product;
                product *= sim3;
                sum += beta3 * product;
                product *= sim4;
                sum += beta4 * product;
                return sum;
            }
            return 0;
        }

        //�������
        //�Ƚ��������ϵ����ƶ�
        public double simList(List<string> list1, List<string> list2)
        {
            if (list1.Count == 0 && list2.Count == 0)
                return 1;
            int m = list1.Count;//��һ���б�ĸ���
            int n = list2.Count;//�ڶ����б�ĸ���
            int big = m > n ? m : n;//�ϴ��
            int N = (m < n) ? m : n;//��С��
            int count = 0;
            int index1 = 0, index2 = 0;//����
            double sum = 0;
            double max = 0;
            while (count < N)
            {
                max = 0;
                for (int i = 0; i < list1.Count; i++)
                {
                    for (int j = 0; j < list2.Count; j++)
                    {
                        //����������ԭ�����ƶȣ��ҵ�������ƶȵĽ�����ԡ�
                        //�������ԭ���ͼ�����ԭ�ľ���
                        //����Ǿ���ʣ���ͬΪ1����ͬΪ0��
                        double sim = innerSimWord(primitive, list1[i], list2[j]);
                        if (sim > max)
                        {
                            index1 = i;
                            index2 = j;
                            max = sim;
                        }
                    }

                }
                //�ۼ�ֵ����
                sum += max;
                //�Ƴ���Գɹ�������
                list1.RemoveAt(index1);
                list2.RemoveAt(index2);
                count++;//��Գɹ��ĸ�����--��ʵ�϶���N���������������Сֵ��
            }
            return (sum + delta * (big - N)) / big;
        }

        //�ڲ��Ƚ������ʣ�������Ϊ����ʣ�Ҳ��������ԭ
        private double innerSimWord(Primitive p, string word1, string word2)
        {
            //Primitive p = new Primitive();
            bool isPrimitive1 = p.isPrimitive(word1);
            bool isPrimitive2 = p.isPrimitive(word2);
            //������ԭ
            if (isPrimitive1 && isPrimitive2)
                return simPrimitive(p, word1, word2);
            if (!isPrimitive1 && !isPrimitive2)
            {
                if (word1.Equals(word2))
                    return 1;
                else
                    return 0;
            }
            // ��ԭ�;���ʵ����ƶ�, Ĭ��Ϊgamma=0.2
            return gamma;
        }

        //����������ԭ֮������ƶ�
        public double simPrimitive(Primitive p, string primitive1, string primitive2)
        {
            int dis = disPrimitive(p, primitive1, primitive2);
            return alpha / (dis + alpha);

        }
        //����������ԭ֮��ľ��룬���������ԭ���û�й�ͬ�ڵ㣬���������ǵľ���Ϊ20��
        public int disPrimitive(Primitive p, string primitive1, string primitive2)
        {
            //Primitive p = new Primitive();
            List<int> list1 = p.getparents(primitive1);
            List<int> list2 = p.getparents(primitive2);
            for (int i = 0; i < list1.Count; i++)
            {
                int id1 = list1[i];
                if (list2.Contains(id1))
                {
                    int index = list2.IndexOf(id1);
                    return index + i;//������ԭ�����ϵĽڵ��·����
                }
            }
            return DEFAULT_PRIMITIVE_DIS;
        }
        //�����ṹDictionary�����ƶ�
        public double simDictionary(Dictionary<string, List<string>> dic1, Dictionary<string, List<string>> dic2)
        {
            if (dic1.Count == 0 && dic2.Count == 0)//�������ṹ��Ϊ�����ƶ�Ϊ1��
            {
                return 1;
            }
            int total = dic1.Count + dic2.Count;
            double sim = 0;
            int count = 0;

            foreach (string key in dic1.Keys)
            {
                System.Console.WriteLine(key);

                if (dic2.ContainsKey(key))
                {
                    //�������Map�е�key��ͬ���ͼ�������Map�����ƶȡ�
                    List<string> list1 = dic1[key];
                    List<string> list2 = dic2[key];
                    sim += simList(list1, list2);
                    count++;
                }
            }
            return (sim + delta * (total - 2 * count)) / (total - count);

        }

        //����һ������
        public void addWord(Word word)
        {
            if (ALLWORDS.ContainsKey(word.getWord()))
            {
                List<Word> list = ALLWORDS[word.getWord()];
                list.Add(word);
            }
            else
            {
                List<Word> list = new List<Word>();
                list.Add(word);
                ALLWORDS.Add(word.getWord(), list);
            }

        }
    }
}
