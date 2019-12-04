using System;
using System.Configuration;
using StackExchange.Redis;

namespace ConsoleRedis
{
    class Program
    {
        static void Main(string[] args)
        {
            string equipe = "DCWA";

            Console.WriteLine("==============================================================================");
            Console.WriteLine("FIAP - ARQUITETURA E DESENVOLVIMENTO NA PLATAFORMA.NET \n");
            Console.WriteLine(" ");
            Console.WriteLine("ARQUITETURA DE BANCO DE DADOS E PERSISTÊNCIA \n");
            Console.WriteLine(" ");
            Console.WriteLine("EQUIPE: " + equipe + "\n");
            Console.WriteLine("==============================================================================");

            //var redis = ConnectionMultiplexer.Connect("localhost:6379");
            //var db = redis.GetDatabase();
            //string valor = db.StringGet("A");
            //Console.WriteLine("A: " + valor);

            string connectionString = ConfigurationManager.AppSettings["connOraculoRedis"];

            var redisSubscriber = ConnectionMultiplexer.Connect(connectionString);
            if (redisSubscriber != null)
                Console.WriteLine("Servidor oráculo ativo no host '" + connectionString+"' \n");

            var db = redisSubscriber.GetDatabase();
            db.StringSet("RP111", "Não faz pergunta dificil");
            db.StringSet("RP112", "Deixa eu pensar");
            db.StringSet("RP113", "Depende!");
            db.StringSet("RP114", "Humm..");
            db.StringSet("RP115", "Vejamos");
            db.StringSet("RP116", "Numsei...");
            db.StringSet("RP118", "Vamos ver..");

            var sub = redisSubscriber.GetSubscriber();
            var channel = "Perguntas";

            Console.WriteLine("Escutando as perguntas do oráculo...");

            sub.Subscribe(channel, (ch, msg) =>
            {
                var pergunta = msg.ToString();

                var idPergunta = pergunta.Substring(0, pergunta.IndexOf(":"));
                string resposta = db.StringGet("R"+idPergunta);

                if (resposta == null)
                    resposta = "Pensando...";

                Console.WriteLine("....Pergunta: " + msg.ToString());
                Console.WriteLine("....Resposta: " + resposta +"\n");

                db.HashSet(idPergunta, equipe, resposta);
            });

            Console.ReadLine();

        }
    }
}

