import argparse
import os
import shutil
import sys

def useLean():
	src = "compileXamarinNativeLibs/standard/pdfnet.aar"
	dst = "pdfnet.aar"
	shutil.copy(src, dst)

def useFull():
	src = "compileXamarinNativeLibs/full/pdfnet.aar"
	dst = "pdfnet.aar"
	shutil.copy(src, dst)

def main():
	parser = argparse.ArgumentParser(description='Choose which library to use')
	parser.add_argument('-f', '--full',help='use full version', action='store_true', dest='build_full')
	stored_args, ignored_args = parser.parse_known_args()

	if stored_args.build_full:
		useFull()
	else:
		useLean()

if __name__ == "__main__":
    main()