using System;
using System.IO;
using UnityEngine;

namespace Graffity.Groq.Speech
{

    public static class WavFileWriter
    {
        public static void FixWavHeader(byte[] originalBytes, string outputPath, int sampleRate = 22050, int channels = 1)
        {
            // "data" チャンクのインデックスを検索
            int dataChunkIndex = FindDataChunkIndex(originalBytes);

            if (dataChunkIndex < 0)
            {
                Debug.LogError("WAVファイル内に 'data' チャンクが見つかりませんでした。");
                return;
            }

            // 実際のPCMデータの開始位置
            int pcmStart = dataChunkIndex + 8; // "data" + 4バイトのサイズ情報

            int pcmDataLength = originalBytes.Length - pcmStart;
            byte[] pcmData = new byte[pcmDataLength];
            Array.Copy(originalBytes, pcmStart, pcmData, 0, pcmDataLength);

            using (FileStream fs = new FileStream(outputPath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // RIFFヘッダ
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + pcmDataLength);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

                // fmtチャンク
                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1); // PCM
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2);
                writer.Write((short)(channels * 2));
                writer.Write((short)16);

                // dataチャンク
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(pcmDataLength);
                writer.Write(pcmData);
            }
        }

        private static int FindDataChunkIndex(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length - 4; i++)
            {
                if (bytes[i] == 'd' && bytes[i + 1] == 'a' && bytes[i + 2] == 't' && bytes[i + 3] == 'a')
                {
                    return i;
                }
            }
            return -1;
        }
    }
}