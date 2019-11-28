using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.IO;
using System;
using UnityEngine;

public class Compresser {
    private MemoryStream buffer;
    private long bits;
    private int currentBitCount;
    public Compresser(){
        buffer = new MemoryStream();
        currentBitCount = 0;
        bits = 0;
    }

    private static int GetBitsRequired(long value) {
        int bitsRequired = 0;
        while (value > 0) {
            bitsRequired++;
            value >>= 1;
        }
        return bitsRequired;
    }
    public Compresser addInt(int val, int min, int max){
        int neededBits = GetBitsRequired(max-min);
        int i = 0;
        while(i < neededBits){
            int bit = (val >> neededBits) & 0b1;
            bits |= bit << currentBitCount;
            currentBitCount++;
            Debug.Log(bits);
        }
        WriteIfNecessary();
        return this;
    }

    public Compresser addFloat(float val, float min, float max, float precision) {
        int neededBits = GetBitsRequired((long) ((max - min) / precision));
        int i = 0;
        int intVal = (int) (val / precision);
        while(i < neededBits){
            int bit = (intVal >> neededBits) & 0b1;
            bits |= bit << currentBitCount;
            currentBitCount++;
            Debug.Log(bits);
        }
        WriteIfNecessary();
        return this;
    }

    public Compresser addBool(bool val) {
        long longValue = val ? 1L : 0L;
        bits |= longValue << currentBitCount;
        currentBitCount++;
        WriteIfNecessary();
        return this;
    }

    private void WriteIfNecessary() {
        if (currentBitCount >= 32) {
            int word = (int) bits;
            byte a = (byte) (word);
            byte b = (byte) (word >> 8);
            byte c = (byte) (word >> 16);
            byte d = (byte) (word >> 24);
            buffer.WriteByte(a);
            buffer.WriteByte(b);
            buffer.WriteByte(c);
            buffer.WriteByte(d);
            bits >>= 32;
            currentBitCount -= 32;
        }
    }

    public byte[] getMessage(){
        byte[] msg = buffer.GetBuffer();
        currentBitCount = 0;
        bits = 0;
        buffer.SetLength(0);
        return msg;
    }
}