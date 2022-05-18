//	Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SavWav {
    
    public static byte[] Save(AudioClip clip) {

        MemoryStream stream = new MemoryStream();
        CreateEmpty(stream, 44);
        ConvertAndWrite(stream, clip);
        WriteHeader(stream, clip);
        return stream.GetBuffer();

    }

    public static AudioClip TrimSilence(AudioClip clip, float min) {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
    }

    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz) {
        return TrimSilence(samples, min, channels, hz, false, false);
    }

    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream) {
        int i;
        for (i = 0; i < samples.Count; i++) {
            if (samples[i] > min) {
                break;
            }
        }

        samples.RemoveRange(0, i);

        for (i = samples.Count - 1; i > 0; i--) {
            if (samples[i] > min) {
                break;
            }
        }

        if (samples.Count > 0) {
            samples.RemoveRange(i, samples.Count - i);
            var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);
            clip.SetData(samples.ToArray(), 0);
            return clip;
        } else {
            return null;
        }
    }

    static void CreateEmpty(MemoryStream mstream, int size) {
        
        byte emptyByte = new byte();

        for (int i = 0; i < size; i++) {
            mstream.WriteByte(emptyByte);
        }        
    }

    static void ConvertAndWrite(MemoryStream mstream, AudioClip clip) {

        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];

        Byte[] bytesData = new Byte[samples.Length * 2];

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++) {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        mstream.Write(bytesData, 0, bytesData.Length);
    }

    static void WriteHeader(MemoryStream mstream, AudioClip clip) {

        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        mstream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        mstream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(mstream.Length - 8);
        mstream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        mstream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        mstream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        mstream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        mstream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        mstream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        mstream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * 2);
        mstream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(2);
        mstream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        mstream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        mstream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * 2);
        mstream.Write(subChunk2, 0, 4);
    }
}