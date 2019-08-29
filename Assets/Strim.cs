using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Strim {
	private long _order = 0L;
	
	// private List observers
	
	private List<Pucket> _puckets;
	private List<Pucket> _acks;
	private bool _reliabilaite;

	public Strim(bool realiabitiliy) {
		_puckets = new List<Pucket>();
		_acks = new List<Pucket>();
		_reliabilaite = realiabitiliy;
	}
	
	public void ReceivePacket(Pucket p) {
		if (p.Ack) {
			_puckets = _puckets.Where(pq => pq.Order > p.Order).ToList();
		} else {
			//observers (p.data)
			if (_reliabilaite) { //crear ack
				CreatePacket(p.Order.ToString(), p.Topic, true);
			}
		}
	}

	public void CreatePacket(string data, int topic, bool ack=false) {
		Pucket q = new Pucket(topic, _order++, data, ack);
		if (ack) {
			_acks.Add(q);
		} else {
			_puckets.Add(q);
		}
	}

	public List<Pucket> GetPockets() {
		List<Pucket> aux = _acks.Concat(_puckets).ToList();
		_acks.Clear();
		return aux;
	}
	
	public class Pucket {
		public int Topic;
		public bool Ack;
		public long Order;
		public string Data;

		public Pucket(int topic, long order, string data, bool ack) {
			Topic = topic;
			Order = order;
			Data = data;
			Ack = ack;
		}
	}
}
