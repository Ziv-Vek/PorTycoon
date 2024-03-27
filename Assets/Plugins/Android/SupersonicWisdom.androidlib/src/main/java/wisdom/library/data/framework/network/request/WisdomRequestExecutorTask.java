package wisdom.library.data.framework.network.request;

import android.text.TextUtils;
import android.util.Log;

import wisdom.library.data.framework.network.utils.NetworkUtils;

import org.json.JSONObject;

import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.Reader;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;
import java.nio.charset.Charset;
import java.util.Map;

import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_INTERNAL_ERROR;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_INTERNAL_IO_ERROR;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_INTERNAL_MALFORMED_URL;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_INTERNAL_NO_INTERNET;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_INTERNAL_PROTOCOL_ERROR;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_INTERNAL_UNSUPPORTED_ENCODING;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_NO_INTERNET_CONNECTION_MSG;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_READ_BUFFER_SIZE;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_REQUEST_ENCODING;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_REQUEST_HEADER_HOST;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_REQUEST_METHOD_GET;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_REQUEST_METHOD_POST;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_RESPONSE_CODE_BAD_REQUEST;
import static wisdom.library.data.framework.network.core.WisdomNetwork.WISDOM_RESPONSE_CODE_OK;

public class WisdomRequestExecutorTask implements Runnable {

    private WisdomRequest mRequest;
    private NetworkUtils mNetworkUtils;

    public WisdomRequestExecutorTask(WisdomRequest request, NetworkUtils networkUtils) {
        mRequest = request;
        mNetworkUtils = networkUtils;
    }

    private void executeRequestAsync(WisdomRequest request) {
        request.startTime = System.currentTimeMillis();
        
        if (!mNetworkUtils.isNetworkAvailable()) {
            request.endTime = System.currentTimeMillis();
            request.onResponseFailed(WISDOM_INTERNAL_NO_INTERNET, WISDOM_NO_INTERNET_CONNECTION_MSG);
            return;
        }

        HttpURLConnection connection = null;
        int responseCode = 0;
        OutputStream outputStream = null;
        BufferedWriter streamWriter = null;
        InputStream inputStream = null;
        InputStreamReader streamReader = null;
        StringBuilder responseSb = new StringBuilder();

        String errorMessage = "";
        try {
            URL url = new URL(request.getUrl());
            connection = (HttpURLConnection) url.openConnection();
            Map<String, String> headers = request.getHeaders();

            connection.setConnectTimeout(request.getConnectTimeout());
            connection.setReadTimeout(request.getReadTimeout());
            connection.setRequestMethod(request.getRequestMethod());
            connection.setRequestProperty(WISDOM_REQUEST_HEADER_HOST, url.getHost());
            for (String key : headers.keySet()) {
                String value = headers.get(key);
                connection.setRequestProperty(key, value);
            }

            connection.setChunkedStreamingMode(0);
            connection.setUseCaches(false);

            if (mRequest.getRequestMethod().equalsIgnoreCase(WISDOM_REQUEST_METHOD_GET)) {
                connection.setDoInput(true);
                connection.setDoOutput(false);
            } else if (mRequest.getRequestMethod().equalsIgnoreCase(WISDOM_REQUEST_METHOD_POST)) {
                connection.setDoInput(true);
                connection.setDoOutput(true);
            }

            JSONObject jsonBody = request.getBody();
            if (connection.getDoOutput() && jsonBody != null && jsonBody.length() > 0) {
                connection.setDoOutput(true);
                outputStream = new BufferedOutputStream(connection.getOutputStream());
                String body = jsonBody.toString();
                streamWriter = new BufferedWriter(new OutputStreamWriter(outputStream, WISDOM_REQUEST_ENCODING));
                streamWriter.write(body);
                streamWriter.flush();
            }

            if (connection.getDoInput()) {
                inputStream = connection.getInputStream();
                streamReader = new InputStreamReader(inputStream);

                try (Reader reader = new BufferedReader(new InputStreamReader
                    (inputStream, Charset.forName(WISDOM_REQUEST_ENCODING)))) {
                    int c = 0;
                    while ((c = reader.read()) != -1) {
                        responseSb.append((char) c);
                    }
                }
            }

        } catch (MalformedURLException e) {
            responseCode = WISDOM_INTERNAL_MALFORMED_URL;
            errorMessage = e.getMessage();
        } catch (UnsupportedEncodingException e) {
            responseCode = WISDOM_INTERNAL_UNSUPPORTED_ENCODING;
            errorMessage = e.getMessage();
        } catch (ProtocolException e) {
            responseCode = WISDOM_INTERNAL_PROTOCOL_ERROR;
            errorMessage = e.getMessage();
        } catch (IOException e) {
            responseCode = WISDOM_INTERNAL_ERROR;
            errorMessage = e.getMessage();
        }

        try {
            if (responseCode == 0) {
                responseCode = connection.getResponseCode();
            }

            if (TextUtils.isEmpty(errorMessage)) {
                errorMessage = connection.getResponseMessage();
            }
        } catch (IOException e) {
            if (responseCode == 0) {
                responseCode = WISDOM_INTERNAL_IO_ERROR;
                errorMessage = e.getMessage();
            }
        }

        closeStreams(streamReader, inputStream, streamWriter, outputStream);
        if (connection != null) {
            connection.disconnect();
        }

        request.endTime = System.currentTimeMillis();
        
        if (responseCode < WISDOM_RESPONSE_CODE_OK || responseCode >= WISDOM_RESPONSE_CODE_BAD_REQUEST) {
            request.onResponseFailed(responseCode, errorMessage);
        } else {
            request.onResponseSuccess(responseSb.toString());
        }
    }

    public int executeRequest() {
        if (!mNetworkUtils.isNetworkAvailable()) {
            return WISDOM_INTERNAL_NO_INTERNET;
        }

        HttpURLConnection connection = null;
        int responseCode = 0;
        OutputStream outputStream = null;
        BufferedWriter streamWriter = null;
        InputStream inputStream = null;
        InputStreamReader streamReader = null;

        try {
            URL url = new URL(mRequest.getUrl());
            connection = (HttpURLConnection) url.openConnection();
            Map<String, String> headers = mRequest.getHeaders();
            connection.setConnectTimeout(mRequest.getConnectTimeout());
            connection.setReadTimeout(mRequest.getReadTimeout());
            connection.setRequestMethod(mRequest.getRequestMethod());
            connection.setRequestProperty(WISDOM_REQUEST_HEADER_HOST, url.getHost());
            for (String key : headers.keySet()) {
                String value = headers.get(key);
                connection.setRequestProperty(key, value);
            }

            connection.setChunkedStreamingMode(0);
            connection.setUseCaches(false);

            if (mRequest.getRequestMethod().equalsIgnoreCase(WISDOM_REQUEST_METHOD_GET)) {
                connection.setDoInput(true);
                connection.setDoOutput(false);
            } else if (mRequest.getRequestMethod().equalsIgnoreCase(WISDOM_REQUEST_METHOD_POST)) {
                connection.setDoOutput(true);
                connection.setDoInput(true);
            }

            JSONObject jsonBody = mRequest.getBody();
            if (connection.getDoOutput() && jsonBody != null && jsonBody.length() > 0) {
                outputStream = new BufferedOutputStream(connection.getOutputStream());
                streamWriter = new BufferedWriter(new OutputStreamWriter(outputStream, WISDOM_REQUEST_ENCODING));
                String body = mRequest.getBody().toString();
                streamWriter.write(body);
                streamWriter.flush();
            }

            if (connection.getDoInput()) {
                inputStream = connection.getInputStream();
                streamReader = new InputStreamReader(inputStream);
                char buff[] = new char[WISDOM_READ_BUFFER_SIZE];
                StringBuilder sb = new StringBuilder();
                while (streamReader.read(buff) > -1) {
                    sb.append(buff);
                }
            }
        } catch (MalformedURLException e) {
            responseCode = WISDOM_INTERNAL_MALFORMED_URL;
            //TODO Add log in future when we will have logger
        } catch (UnsupportedEncodingException e) {
            responseCode = WISDOM_INTERNAL_UNSUPPORTED_ENCODING;
            //TODO Add log in future when we will have logger
        } catch (ProtocolException e) {
            responseCode = WISDOM_INTERNAL_PROTOCOL_ERROR;
            //TODO Add log in future when we will have logger
        } catch (IOException e) {
            responseCode = WISDOM_INTERNAL_IO_ERROR;
            //TODO Add log in future when we will have logger
        }

        try {
            responseCode = connection.getResponseCode();
        } catch (IOException e) {
            if (responseCode == 0) {
                responseCode = WISDOM_INTERNAL_IO_ERROR;
            }
        } finally {
            closeStreams(streamReader, inputStream, streamWriter, outputStream);
            if (connection != null) {
                connection.disconnect();
            }

            return responseCode;
        }
    }

    private void closeStreams(InputStreamReader reader,
                              InputStream inputStream,
                              BufferedWriter writer,
                              OutputStream outputStream) {
        try {
            if (reader != null) {
                reader.close();
            }

            if (inputStream != null) {
                inputStream.close();
            }

            if (writer != null) {
                writer.close();
            }

            if (outputStream != null) {
                outputStream.close();
            }
        } catch (IOException ex) {
            //TODO Add log in future
        }
    }

    @Override
    public void run() {
        executeRequestAsync(mRequest);
    }
}
