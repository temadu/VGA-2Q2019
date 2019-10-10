using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System;
using UnityEngine;

public class Strim {
	private long _order = 0L;

	private List<Action<string>> _observers;
	
	private List<Pucket> _puckets;
	private List<Pucket> _acks;
	private bool _reliabilaite;

	public Strim(bool realiabitiliy) {
		_puckets = new List<Pucket>();
		_acks = new List<Pucket>();
		_observers = new List<Action<string>>();
		_reliabilaite = realiabitiliy;
	}
	
	public void ReceivePacket(Pucket p) {
		if (p.Ack) {
			_puckets = _puckets.Where(pq => pq.Order > p.Order).ToList();
		} else {
			Debug.Log(p.Data);
			_observers.ForEach(obs=> obs(p.Data));
			if (_reliabilaite) { //crear ack
				CreatePacket(p.Order.ToString(), p.Topic, true);
			}
		}
	}

	public Pucket CreatePacket(string data, int topic, bool ack=false) {
		Pucket q = new Pucket(topic, _order++, data, ack);
		if (ack) {
			_acks.Add(q);
		} else {
			_puckets.Add(q);
		}

		return q;
	}

	public void addObserver(Action<string> obs) {
		_observers.Add(obs);
	}

	public List<Pucket> GetPockets() {
		List<Pucket> aux = _acks.Concat(_puckets).ToList();
		_acks.Clear();
		return aux;
	}
}
