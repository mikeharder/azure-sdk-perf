from tornado import httpclient

from azure.test.perfstress import PerfStressTest

class TornadoGetTest(PerfStressTest):

    async def GlobalSetupAsync(self):
        httpclient.AsyncHTTPClient.configure("tornado.curl_httpclient.CurlAsyncHTTPClient")
        type(self).client = httpclient.AsyncHTTPClient()

    async def RunAsync(self):
        await type(self).client.fetch(self.Arguments.url)

    @staticmethod
    def AddArguments(parser):
        parser.add_argument('-u', '--url', required=True)
