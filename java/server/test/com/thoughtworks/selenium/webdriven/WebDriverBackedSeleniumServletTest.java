package com.thoughtworks.selenium.webdriven;


import static org.junit.Assert.assertTrue;

import com.thoughtworks.selenium.DefaultSelenium;
import com.thoughtworks.selenium.Selenium;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import org.openqa.selenium.Pages;
import org.openqa.selenium.environment.GlobalTestEnvironment;
import org.openqa.selenium.environment.InProcessTestEnvironment;
import org.openqa.selenium.environment.TestEnvironment;
import org.openqa.selenium.environment.webserver.AppServer;
import org.openqa.selenium.net.PortProber;
import org.openqa.selenium.remote.server.DefaultDriverSessions;
import org.openqa.selenium.remote.server.DriverServlet;
import org.seleniumhq.jetty9.server.Connector;
import org.seleniumhq.jetty9.server.HttpConfiguration;
import org.seleniumhq.jetty9.server.HttpConnectionFactory;
import org.seleniumhq.jetty9.server.Server;
import org.seleniumhq.jetty9.server.ServerConnector;
import org.seleniumhq.jetty9.servlet.ServletContextHandler;

public class WebDriverBackedSeleniumServletTest {

  private Server server;
  private int port;
  private AppServer appServer;
  private Pages pages;

  @Before
  public void setUpServer() throws Exception {
    server = new Server();

    // Register the emulator
    ServletContextHandler handler = new ServletContextHandler();

    DefaultDriverSessions webdriverSessions = new DefaultDriverSessions();
    handler.setAttribute(DriverServlet.SESSIONS_KEY, webdriverSessions);
    handler.setContextPath("/");
    handler.addServlet(WebDriverBackedSeleniumServlet.class, "/selenium-server/driver/");
    server.setHandler(handler);

    // And bind a port
    port = PortProber.findFreePort();
    HttpConfiguration httpConfig = new HttpConfiguration();
    ServerConnector http = new ServerConnector(server, new HttpConnectionFactory(httpConfig));
    http.setPort(port);
    server.setConnectors(new Connector[]{http});

    // Wait until the server is actually started.
    server.start();
    PortProber.pollPort(port);
  }

  @Before
  public void prepTheEnvironment() {
    TestEnvironment environment = GlobalTestEnvironment.get(InProcessTestEnvironment.class);
    appServer = environment.getAppServer();

    pages = new Pages(appServer);
  }

  @After
  public void stopServer() throws Exception {
    if (server != null) {
      server.stop();
    }
  }

  @Test
  public void searchGoogle() {
    Selenium selenium = new DefaultSelenium("localhost", port, "*firefox", appServer.whereIs("/"));
    selenium.start();

    selenium.open(pages.simpleTestPage);
    String text = selenium.getBodyText();

    selenium.stop();
    assertTrue(text.contains("More than one line of text"));
  }
}