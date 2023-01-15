import { AxiosResponseHeaders, RawAxiosResponseHeaders } from 'axios';
import { dataURLtoBlob, urlToBase64 } from './base64Conver';


/**
 * Download online pictures
 * @param url
 * @param filename
 * @param mime
 * @param bom
 */
export function downloadByOnlineUrl(url: string, filename: string, mime?: string, bom?: BlobPart) {
	urlToBase64(url).then((base64) => {
		downloadByBase64(base64, filename, mime, bom);
	});
}

/**
 * Download pictures based on base64
 * @param buf
 * @param filename
 * @param mime
 * @param bom
 */
export function downloadByBase64(buf: string, filename: string, mime?: string, bom?: BlobPart) {
	const base64Buf = dataURLtoBlob(buf);
	downloadByData(base64Buf, filename, mime, bom);
}

/**
 * Download according to the background interface file stream
 * @param {*} data
 * @param {*} filename
 * @param {*} mime
 * @param {*} bom
 */
export function downloadByData(data: BlobPart, filename: string, mime?: string, bom?: BlobPart) {
	const blobData = typeof bom !== 'undefined' ? [bom, data] : [data];
	const blob = new Blob(blobData, { type: mime || 'application/octet-stream' });

	const blobURL = window.URL.createObjectURL(blob);
	const tempLink = document.createElement('a');
	tempLink.style.display = 'none';
	tempLink.href = blobURL;
	tempLink.setAttribute('download', filename);
	if (typeof tempLink.download === 'undefined') {
		tempLink.setAttribute('target', '_blank');
	}
	document.body.appendChild(tempLink);
	tempLink.click();
	document.body.removeChild(tempLink);
	window.URL.revokeObjectURL(blobURL);
}

/**
 * Download file according to file address
 * @param {*} sUrl
 */
export function downloadByUrl({ url, target = '_blank', fileName }: { url: string; target?: TargetContext; fileName?: string }): boolean {
	const isChrome = window.navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
	const isSafari = window.navigator.userAgent.toLowerCase().indexOf('safari') > -1;

	if (/(iP)/g.test(window.navigator.userAgent)) {
		console.error('Your browser does not support download!');
		return false;
	}
	if (isChrome || isSafari) {
		const link = document.createElement('a');
		link.href = url;
		link.target = target;

		if (link.download !== undefined) {
			link.download = fileName || url.substring(url.lastIndexOf('/') + 1, url.length);
		}

		if (document.createEvent) {
			const e = document.createEvent('MouseEvents');
			e.initEvent('click', true, true);
			link.dispatchEvent(e);
			return true;
		}
	}
	if (url.indexOf('?') === -1) {
		url += '?download';
	}

	openWindow(url, { target });
	return true;
}

export function openWindow(url: string, opt?: { target?: TargetContext | string; noopener?: boolean; noreferrer?: boolean }) {
	const { target = '__blank', noopener = true, noreferrer = true } = opt || {};
	const feature: string[] = [];

	noopener && feature.push('noopener=yes');
	noreferrer && feature.push('noreferrer=yes');

	window.open(url, target, feature.join(','));
}

export function getFileName(headers: RawAxiosResponseHeaders | AxiosResponseHeaders) {
	var fileName = headers['content-disposition'].split(';')[1].split('filename=')[1];
	var fileNameUnicode = headers['content-disposition'].split('filename*=')[1];
	if (fileNameUnicode) {
		//当存在 filename* 时，取filename* 并进行解码（为了解决中文乱码问题）
		fileName = decodeURIComponent(fileNameUnicode.split("''")[1]);
	}
	return fileName;
}
