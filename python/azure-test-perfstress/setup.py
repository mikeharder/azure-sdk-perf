from setuptools import find_packages, setup

PACKAGE_NAME = "azure-test-perfstress"
PACKAGE_PPRINT_NAME = "Perf/Stress"

setup(
    name=PACKAGE_NAME,
    version="0.0.1",
    description='Microsoft Azure {} Library for Python'.format(PACKAGE_PPRINT_NAME),
    license='MIT License',
    author='Microsoft Corporation',
    author_email='azpysdkhelp@microsoft.com',
    url='https://github.com/mikeharder/StoragePerf/tree/master/python',
    classifiers=[
        'Development Status :: 5 - Production/Stable',
        'Programming Language :: Python',
        'Programming Language :: Python :: 2',
        'Programming Language :: Python :: 2.7',
        'Programming Language :: Python :: 3',
        'Programming Language :: Python :: 3.5',
        'Programming Language :: Python :: 3.6',
        'Programming Language :: Python :: 3.7',
        'Programming Language :: Python :: 3.8',
        'License :: OSI Approved :: MIT License',
    ],
    packages=find_packages()
)