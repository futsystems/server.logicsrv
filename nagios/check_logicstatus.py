#!/usr/bin/env python
# -*- coding: utf-8 -*-

import os
import sys
import argparse
import logging
import nagiosplugin
import json
import requests


def status_request():
    response = requests.post(
        "http://127.0.0.1:8080/api/",
        data={"method": "status",})

    return response.status_code , response.json()


class LogicStatusCheck(nagiosplugin.Resource):
    """
    used to check logic status
    """
    def __init__(self, address, reqport):
        self.module = 'MgrExchServer'
        self.method = 'qrystatus'
        self.address = address
        self.reqport = reqport
        self.data=None

    def probe(self):
        try:
            status,reply = status_request()
            if reply['RspCode'] == 0:
                return [
                    nagiosplugin.Metric('MGR', reply['Payload']['ManagerNum'], min=0, context='perf'),
                    nagiosplugin.Metric('ACCT', reply['Payload']['AccountNum'], min=0, context='perf'),
                    nagiosplugin.Metric('ORDER', reply['Payload']['OrderNum'], min=0, context='perf'),
                    nagiosplugin.Metric('TRADE', reply['Payload']['TradeNum'], min=0, context='perf'),
                    # nagiosplugin.Metric('MEMORY',reply.payload['MemSize'],min=0,context='perf'),
                    # nagiosplugin.Metric('CPUTime',reply.payload['CPUTime'],min=0,context='perf'),
                ]
            else:
                raise nagiosplugin.CheckError(reply['Message'])
        except IOError:
            raise nagiosplugin.CheckError("request to socket timeout")


class LogicStatusSummary(nagiosplugin.Summary):
    def  ok(self, results):
        return str(results['MGR'].metric)

    def problem(self, results):
        return super(LogicStatusSummary,self).problem(results)

def main():
    print 'start main'
    argp = argparse.ArgumentParser()
    argp.add_argument('-H', '--host',default='127.0.0.1',
                    help='logic server address'),
    argp.add_argument('-P', '--port',default=8080,
                    help='logc api http port'),
    argp.add_argument('-C', '--command',default='status',
                    help='command to request'),
    argp.add_argument('-a', '--alertstatus', default=True,
                    help='critical if massalert status match'),
    argp.add_argument('-d', '--datafeedstatus',default=False,
                    help='critical if datafeed status match')
    argp.add_argument('-v', '--verbose', action='count', default=0,
                    help='increase output verbosity (use up to 3 times)')
    argp.add_argument('-t', '--timeout', default=10,
                    help='abort execution after TIMEOUT seconds')
    args = argp.parse_args()
    check = nagiosplugin.Check(
        LogicStatusCheck(args.host,args.port),
        nagiosplugin.ScalarContext('mgrnum',0,5),
        nagiosplugin.ScalarContext('perf'),
        LogicStatusSummary()
    )
    check.main(args.verbose, args.timeout)

if __name__ == '__main__':
  main()