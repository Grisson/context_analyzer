using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Work
{
    //���ƶȡ���������
    public class Word
    {
        private string word;//���ﱾ��
        private string type;//��������ADJ NUM N PREP��
        private string related;

        //��һ������ԭ��
        private string firstPrimitive;
        //����������ԭ��  --���˻�����ԭ֮��ģ����ǰ��û�з���#$@!�ȣ�Ҳ������=����ԭ��
        private List<string> otherPrimitives = new List<string>();
        //�����list�ǿգ���ô���һ����ʡ� �б����ŵ��Ǹ���ʵ�һ����ԭ�����������������ʽ���
        private List<string> structruralWords = new List<string>();
        //�ôʵĹ�ϵ��ԭ��key: ��ϵ��ԭ�� value�� ������ԭ|(�����)��һ���б�
        private Dictionary<string, List<string>> relationalPrimitives = new Dictionary<string, List<string>>();
        //�ôʵĹ�ϵ������ԭ��Key: ��ϵ���š� value: ���ڸù�ϵ���ŵ�һ�������ԭ|(�����)
        private Dictionary<string, List<string>> relationSimbolPrimitives = new Dictionary<string, List<string>>();
        //��ȡ���ﱾ��
        public string getWord()
        {
            return word;
        }
        public string getRelated()
        {
            return related;
        }
        public void setRelated(string related)
        {
            this.related = related;
        }
        //���ô���
        public void setWord(string word)
        {
            this.word = word;
        }
        //��ȡ��������
        public string getType()
        {
            return type;
        }
        //���ô�������--����--Part of Speech
        public void setType(string type)
        {
            this.type = type;
        }
        //��ȡ��һ����ԭ
        public string getFirstPrimitive()
        {
            return firstPrimitive;
        }
        //���õ�һ����ԭ
        public void setFirstPrimitive(string firstPrimitive)
        {
            this.firstPrimitive = firstPrimitive;
        }
        //��ȡ������ԭ
        public List<string> getOtherPrimitives()
        {
            return otherPrimitives;
        }
        //����������ԭ
        public void setOtherPrimitives(List<string> otherPrimitives)
        {
            this.otherPrimitives = otherPrimitives;
        }
        //���������ԭ����List<string>��������string���͵� <<��ԭ>>
        public void addOtherPrimitive(string otherPrimitive)
        {
            this.otherPrimitives.Add(otherPrimitive);
        }
        //��ȡ�ṹ��ԭ
        public List<string> getStructruralWords()
        {
            return structruralWords;
        }
        //�Ƿ�Ϊ���--�������ʣ���structruralWords�ǿա�
        public bool isStructruralWord()
        {
            return !(structruralWords.Count == 0);
        }
        //���ýṹ��ԭ
        public void setStructruralWords(List<string> structruralWords)
        {
            this.structruralWords = structruralWords;
        }
        //��ӽṹ��ԭ
        public void addStructruralWord(string structruralWord)
        {
            this.structruralWords.Add(structruralWord);
        }
        //��ȡ��ϵ��ԭ
        public Dictionary<string, List<string>> getRelationalPrimitives()
        {
            return relationalPrimitives;
        }
        //��ȡ��ϵ������ԭ
        public Dictionary<string, List<string>> getRelationSimbolPrimitives()
        {
            return relationSimbolPrimitives;
        }
        //��ӹ�ϵ��ԭ
        //�����ϵ��ԭ��key��Ӧ��ListΪ�գ����½�һ��������value��
        //���򣬾�ֱ���ڹ�ϵ��ԭ��key��Ӧ��List����ֱ������value��
        public void addRelationalPrimitive(string key, string value)
        {
            List<string> list = null;
            if (relationalPrimitives.ContainsKey(key))
            {
                list = relationalPrimitives[key];
                list.Add(value);
            }
            else {
                list = new List<string>();
                list.Add(value);
                relationalPrimitives.Add(key, list);
            }
        }
        //��ӽṹ������ԭ
        public void addRelationSimbolPrimitive(string key, string value)
        {
            List<string> list = null;
            if (relationSimbolPrimitives.ContainsKey(key))
            {
                list = relationSimbolPrimitives[key];
                list.Add(value);
            }
            else
            {
                list = new List<string>();
                list.Add(value);
                relationSimbolPrimitives.Add(key, list);
            }
           
        }

    }
}
